using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinwheel.PolarisBasic
{
    /// <summary>
    /// Bezier based vertex deformation processor
    /// </summary>
    [System.Serializable]
    public partial class VertexWarper
    {
        /// <summary>
        /// Fired when any property of its beziers changed
        /// </summary>
        /// <param name="beziers"></param>
        public delegate void BezierChangedHandler(Bezier[] beziers);
        public event BezierChangedHandler BezierChanged;

        public const int CONTROL_POINT_COUNT = 4;

        /// <summary>
        /// Should it fire event when beziers modified?
        /// </summary>
        private bool modifySilently;
        public bool ModifySilently
        {
            get
            {
                return modifySilently;
            }
            set
            {
                modifySilently = value;
            }
        }

        /// <summary>
        /// Y coordinate to draw Scene view GUI
        /// </summary>
        private float y;
        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        private float minX;
        public float MinX
        {
            get
            {
                return minX;
            }
            set
            {
                float oldValue = minX;
                float newValue = value;
                minX = value;
                if (oldValue != newValue)
                    needReAlign = true;
            }
        }

        private float maxX;
        public float MaxX
        {
            get
            {
                return maxX;
            }
            set
            {
                float oldValue = maxX;
                float newValue = value;
                maxX = value;
                if (oldValue != newValue)
                    needReAlign = true;
            }
        }

        private float minZ;
        public float MinZ
        {
            get
            {
                return minZ;
            }
            set
            {
                float oldValue = minZ;
                float newValue = value;
                minZ = value;
                if (oldValue != newValue)
                    needReAlign = true;
            }
        }

        private float maxZ;
        public float MaxZ
        {
            get
            {
                return maxZ;
            }
            set
            {
                float oldValue = maxZ;
                float newValue = value;
                maxZ = value;
                if (oldValue != newValue)
                    needReAlign = true;
            }
        }

        public float SizeX
        {
            get
            {
                return Mathf.Abs(MaxX - MinX);
            }
        }

        public float SizeZ
        {
            get
            {
                return Mathf.Abs(MaxZ - MinZ);
            }
        }

        private Bezier[] beziers;
        public Bezier[] Beziers
        {
            get
            {
                return beziers;
            }
            private set
            {
                if (value != null && value.Length != 4)
                    throw new ArgumentException("Invalid Beziers collection, only accept 4 elements.");
                beziers = value;
            }
        }

        /// <summary>
        /// Transform of the terrain it associates to
        /// </summary>
        private Transform transform;
        public Transform Transform
        {
            get
            {
                return transform;
            }
            set
            {
                transform = value;
            }
        }
        
        private Bezier.ControlPoint[] controlPoints;

#pragma warning disable 0414
        private bool needReAlign;
#pragma warning restore 0414

        private void CreateDefaultBeziers()
        {
            ApplyTemplate(TemplateMaker.GetDefaultTemplate(Y, MinX, MaxX, MinZ, MaxZ));
        }

        private void RegisterBezierChangedEvent()
        {
            for (int i = 0; i < Beziers.Length; ++i)
            {
                Beziers[i].ControlPointsChanged += OnBezierControlPointsChanged;
            }
        }

        private void OnBezierControlPointsChanged(List<Bezier.ControlPoint> poinst)
        {
            if (BezierChanged != null && !ModifySilently)
            {
                BezierChanged(Beziers);
            }
        }

        /// <summary>
        /// Warp a vertex
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Warp(ref float x, ref float y, ref float z)
        {
            float tX = Utilities.GetFraction(x, MinX, MaxX);
            float tZ = Utilities.GetFraction(z, MinZ, MaxZ);

            Vector3 x0Original = new Vector3(x, y, MinZ);
            Vector3 x0 = Beziers[3].GetPoint(1 - tX);
            Vector3 x0Offset = (x0 - x0Original) * (1 - tZ);

            Vector3 x1Original = new Vector3(x, y, MaxZ);
            Vector3 x1 = Beziers[1].GetPoint(tX);
            Vector3 x1Offset = (x1 - x1Original) * tZ;

            Vector3 z0Original = new Vector3(MinX, y, z);
            Vector3 z0 = Beziers[0].GetPoint(tZ);
            Vector3 z0Offset = (z0 - z0Original) * (1 - tX);

            Vector3 z1Original = new Vector3(MaxX, y, z);
            Vector3 z1 = Beziers[2].GetPoint(1 - tZ);
            Vector3 z1Offset = (z1 - z1Original) * tX;

            Vector3 origin = new Vector3(x, y, z);
            Vector3 quadDeformOffset = origin - QuadInterpolateXZ(
                controlPoints[0].Position,
                controlPoints[1].Position,
                controlPoints[2].Position,
                controlPoints[3].Position,
                tX, tZ);
            Vector3 p = origin + x0Offset + x1Offset + z0Offset + z1Offset + quadDeformOffset;

            x = p.x;
            z = p.z;
        }

        /// <summary>
        /// Calculate the warped position depend on 4 control points, ignore the curves
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="tX"></param>
        /// <param name="tZ"></param>
        /// <returns></returns>
        private Vector3 QuadInterpolateXZ(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float tX, float tZ)
        {
            Vector3 x0 = Vector3.Lerp(p0, p3, tX);
            Vector3 x1 = Vector3.Lerp(p1, p2, tX);
            Vector3 z0 = Vector3.Lerp(p0, p1, tZ);
            Vector3 z1 = Vector3.Lerp(p3, p2, tZ);

            Line2D lineX = new Line2D(x0.x, x0.z, x1.x, x1.z);
            Line2D lineZ = new Line2D(z0.x, z0.z, z1.x, z1.z);

            Vector2 intersect = Vector2.zero;
            Line2D.Intersect(lineX, lineZ, out intersect);

            return new Vector3(intersect.x, p0.y, intersect.y);
        }

        /// <summary>
        /// Store beziers data
        /// </summary>
        /// <param name="data"></param>
        public void Serialize(VertexWarper.SerializeData data)
        {
            if (data == null)
                return;
            data.modifySilently = ModifySilently;
            data.y = Y;
            data.minX = MinX;
            data.maxX = MaxX;
            data.minZ = MinZ;
            data.maxZ = MaxZ;

            if (controlPoints == null || controlPoints.Length == 0)
            {
                data.controlPointsInfo = null;
                return;
            }

            data.controlPointsInfo = new Vector3[CONTROL_POINT_COUNT * Bezier.ControlPoint.PROPERTY_COUNT];
            for (int i = 0; i < data.controlPointsInfo.Length; i += Bezier.ControlPoint.PROPERTY_COUNT)
            {
                data.controlPointsInfo[i] = controlPoints[i / Bezier.ControlPoint.PROPERTY_COUNT].Position;
                data.controlPointsInfo[i + 1] = controlPoints[i / Bezier.ControlPoint.PROPERTY_COUNT].Handles[0];
                data.controlPointsInfo[i + 2] = controlPoints[i / Bezier.ControlPoint.PROPERTY_COUNT].Handles[1];
            }
        }

        /// <summary>
        /// Load saved beziers data
        /// </summary>
        /// <param name="data"></param>
        public void DeSerialize(VertexWarper.SerializeData data)
        {
            if (data == null || data.controlPointsInfo == null || data.controlPointsInfo.Length == 0)
                return;
            ModifySilently = data.modifySilently;
            Y = data.y;
            MinX = data.minX;
            MaxX = data.maxX;
            MinZ = data.minZ;
            MaxZ = data.maxZ;

            ApplyTemplate(data.controlPointsInfo);
        }

        /// <summary>
        /// Apply a warping template
        /// </summary>
        /// <param name="template"></param>
        public void ApplyTemplate(Template template)
        {
            ApplyTemplate(TemplateMaker.GetTemplate(template, Y, MinX, MaxX, MinZ, MaxZ));
        }

        public void ApplyTemplate(Vector3[] template)
        {
            if (template == null || template.Length == 0)
                return;
            controlPoints = new Bezier.ControlPoint[CONTROL_POINT_COUNT];
            for (int i = 0; i < controlPoints.Length; ++i)
            {
                controlPoints[i] = new Bezier.ControlPoint(
                    template[i * Bezier.ControlPoint.PROPERTY_COUNT + 0],
                    template[i * Bezier.ControlPoint.PROPERTY_COUNT + 1],
                    template[i * Bezier.ControlPoint.PROPERTY_COUNT + 2]);
            }

            Bezier[] b = new Bezier[CONTROL_POINT_COUNT];
            for (int i = 0; i < b.Length; ++i)
            {
                b[i] = new Bezier(controlPoints[i], controlPoints[(i + 1) % b.Length]);
            }
            Beziers = b;
            RegisterBezierChangedEvent();
        }

        public void DrawSceneGUI()
        {
#if UNITY_EDITOR
            DrawHandles();
            DrawOverlayInstruction();
#endif
        }

        private void DrawHandles()
        {
#if UNITY_EDITOR
            if (Beziers == null)
                return;
            Vector3 p0 = new Vector3(MinX, Y, MinZ);
            Vector3 p1 = new Vector3(MinX, Y, MaxZ);
            Vector3 p2 = new Vector3(MaxX, Y, MaxZ);
            Vector3 p3 = new Vector3(MaxX, Y, MinZ);
            Handles.color = Color.green;
            Handles.DrawDottedLines(new Vector3[] { p0, p1, p1, p2, p2, p3, p3, p0 }, 5);

            for (int i = 0; i < Beziers.Length; ++i)
            {
                Beziers[i].DrawHandles();
            }

            if (Transform == null || Transform.position == Vector3.zero)
                return;

            Handles.color = Color.green;
            Handles.DrawDottedLine(Vector3.zero, Transform.position, 5);
            Handles.Label(
                Vector3.zero,
                "Vertex warping tool is intentionally\n fixed at world origin.",
                GuiStyleUtilities.HandlesLabelStyle);
#endif
        }

        private void DrawOverlayInstruction()
        {
#if UNITY_EDITOR
            Vector2 guiSize = new Vector2(167, 75);
            Vector2 guiPosition = new Vector2(Screen.width - guiSize.x, Screen.height - guiSize.y);
            Rect r = new Rect(guiPosition, guiSize);
            Handles.BeginGUI();
            GUILayout.BeginArea(r);
            bool isCtrlHold = Event.current != null && Event.current.control;
            bool isShiftHold = Event.current != null && Event.current.shift;
            string ctrlText = isCtrlHold ? "<b>Ctrl</b>" : "Ctrl";
            string shiftText = isShiftHold ? "<b>Shift</b>" : "Shift";
            string instruction = string.Format(
                "Hold {0} for snapping\nHold {1} for group editing",
                shiftText,
                ctrlText);
            GUILayout.Box(instruction, GuiStyleUtilities.OverlayInstructionStyle);
            GUILayout.EndArea();
            Handles.EndGUI();
#endif
        }
    }
}
