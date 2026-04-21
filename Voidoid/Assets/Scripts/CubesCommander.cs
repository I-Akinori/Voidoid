using System;
using UnityEngine;

namespace Voidoid
{
    public class CubesCommander: MonoBehaviour
    {
        [SerializeField] private CubeController[] cube;
        [SerializeField] private float[] timeStamp;
        private Action<float>[] action;
        private Vector3[] beforePos;
        private Quaternion[] beforeRot;
        private Vector3[] localPos;
        private Quaternion[] localRot;
        private Vector3[] initPos;
        private Quaternion[] initRot;
        private int index;

        private void Start()
        {
            action = new Action<float>[timeStamp.Length];
            action[0] = Idle;
            action[1] = Separation;
            action[2] = RotMove;
            action[3] = FormTorus;
            action[4] = RotTorus;
            action[5] = StayTorus;
            action[6] = StayTorus;
            action[7] = RotTorus2;
            action[8] = SeparateTorus;
            action[9] = Align;
            action[10] = RotCube;
            action[11] = Return;
            action[12] = Idle;
            action[13] = Idle;
            //action[13] = Idle;
            beforePos = new Vector3[cube.Length];
            beforeRot = new Quaternion[cube.Length];
            localPos = new Vector3[cube.Length];
            localRot = new Quaternion[cube.Length];
            initPos = new Vector3[cube.Length];
            initRot = new Quaternion[cube.Length];
            for (var i = 0; i < cube.Length; i++)
            {
                initPos[i] = cube[i].Position;
                initRot[i] = cube[i].Rotation;
            }
        }

        public void Reset()
        {
            index = 0;
        }

        private void Update()
        {
            if (index == timeStamp.Length - 1)
            {
                // 最後のイベント
                return;
            }

            var duration = timeStamp[index + 1] - timeStamp[index];
            action[index]((TimelineManager.Instance.Time - timeStamp[index]) / duration);
            
            if (TimelineManager.Instance.Time >= timeStamp[index + 1])
            {
                index++;
                for (var i = 0; i < cube.Length; i++)
                {
                    beforePos[i] = cube[i].Position;
                    beforeRot[i] = cube[i].Rotation;
                }
            }
        }

        private void Idle(float time)
        {
        }

        private void Separation(float time)
        {
            if (time < 0.5f)
            {
                for (var i = 1; i < 4; i++)
                {
                    var c = cube[i];
                    var t = time * 2;
                    if (t > (i - 1) / 3f)
                    {
                        c.SetActiveBottom(true);
                        cube[i - 1].SetActiveTop(true);
                    }
                    if (t > i / 3f)
                    {
                        continue;
                    }
                    var s = c.Scale.x;
                    var e0 = i * s * 1f + s * 0.5f;
                    var e1 = e0 + 1.5f * s;
                    var p = c.Position;
                    p.y = Mathf.Lerp(e0, e1, t);
                    c.SetPosition(p);
                }
            }
            else
            {
                for (var i = 1; i < 4; i++)
                {
                    var c = cube[i + 4];
                    var t = (time - 0.5f) * 2;
                    if (t > (i - 1) / 3f)
                    {
                        c.SetActiveBottom(true);
                        cube[i + 3].SetActiveTop(true);
                    }
                    if (t > i / 3f)
                    {
                        continue;
                    }
                    var s = c.Scale.x;
                    var e0 = i * s * 1f + s * 0.5f;
                    var e1 = e0 + 1.5f * s;
                    var p = c.Position;
                    p.y = Mathf.Lerp(e0, e1, t);
                    c.SetPosition(p);
                }
            }
        }
        private void RotMove(float time)
        {
            for (var i = 0; i < 8; i++)
            {
                var dt = 1f / 10;
                var t = Mathf.Clamp01((time - dt * i) / (2 * dt));
                var c = cube[i];
                var s = c.Scale.x;
                var axisY = new Vector3(0 + 0.03f + s * 0.5f, 0, 4.9f - s * 0.5f);
                var p = c.Position;
                var p2d = new Vector2(p.x - axisY.x, p.z - axisY.z);
                var r = p2d.magnitude;
                p.x = r * Mathf.Sin(t * Mathf.PI * 0.5f) + axisY.x;
                p.z = - r * Mathf.Cos(t * Mathf.PI * 0.5f) + axisY.z;
                c.SetPosition(p);
                c.SetRotation(Quaternion.Euler(0, -90 * t, 0));
            }
        }

        private void FormTorus(float time)
        {
            for (var i = 0; i < 8; i++)
            {
                var dc = 4;
                var dt = 1f / (8 + 1 + dc);
                var t = Mathf.Clamp01((time - dt * (7 - i)) / (dc * dt));
                var c = cube[i];
                var bp = beforePos[i];
                var br = beforeRot[i];
                var axisX = new Vector3(1, 1, 4);
                var p = c.Position;
                var rad = 0.75f;
                var theta = Mathf.PI * (i * 0.25f + time * 2 + 0.5f);
                var lp = new Vector3(0, rad * Mathf.Sin(theta), rad * Mathf.Cos(theta));
                p = lp + axisX;
                c.SetPosition(Vector3.Lerp(bp, p, t));
                c.SetRotation(Quaternion.Slerp(br, Quaternion.Euler(0, -90, 180 * (i * 0.25f + time * 2f + 0.5f)), t));
                localPos[i] = lp;
                localRot[i] = Quaternion.Euler(0, -90, 180 * (i * 0.25f + 0.5f));
            }
        }
        private void RotTorus(float time)
        {
            for (var i = 0; i < 8; i++)
            {
                var c = cube[i];
                var rot = Quaternion.Euler(0, 45 * time, 0) * Quaternion.Euler(- 360 * time, 0, 0);
                var parentM = Matrix4x4.TRS(new Vector3(1, 1, 4), rot, Vector3.one);
                var localM  = Matrix4x4.TRS(localPos[i], localRot[i], c.Scale);
                var worldM = parentM * localM;
                c.SetPosition(worldM.GetPosition());
                c.SetRotation(worldM.rotation);
            }
        }
        private void StayTorus(float time)
        {
            for (var i = 0; i < 8; i++)
            {
                var c = cube[i];
                var rot = Quaternion.Euler(0, 45, 0) * Quaternion.Euler(- 360 * time, 0, 0);
                var parentM = Matrix4x4.TRS(new Vector3(1, 1, 4), rot, Vector3.one);
                var localM  = Matrix4x4.TRS(localPos[i], localRot[i], c.Scale);
                var worldM = parentM * localM;
                c.SetPosition(worldM.GetPosition());
                c.SetRotation(worldM.rotation);
            }
        }
        private void RotTorus2(float time)
        {
            for (var i = 0; i < 8; i++)
            {
                var c = cube[i];
                var rot = Quaternion.Euler(0, 45 * (1 + time), 0) * Quaternion.Euler(- 360 * time, 0, 0);
                var parentM = Matrix4x4.TRS(new Vector3(1, 1, 4), rot, Vector3.one);
                var localM  = Matrix4x4.TRS(localPos[i], localRot[i], c.Scale);
                var worldM = parentM * localM;
                c.SetPosition(worldM.GetPosition());
                c.SetRotation(worldM.rotation);
            }
        }
        private void SeparateTorus(float time)
        {
            for (var i = 0; i < 8; i++)
            {
                localPos[i].x = (time * Mathf.Sin(Mathf.PI * 0.25f) * 0.75f) * (i % 2 == 0 ? 1 : -1);
                var c = cube[i];
                var rot = Quaternion.Euler(0, 90, 0) * Quaternion.Euler(- 360 * time, 0, 0);
                var parentM = Matrix4x4.TRS(new Vector3(1, 1, 4), rot, Vector3.one);
                var localM  = Matrix4x4.TRS(localPos[i], localRot[i], c.Scale);
                var worldM = parentM * localM;
                c.SetPosition(worldM.GetPosition());
                c.SetRotation(worldM.rotation);
            }
        }
        private void Align(float time)
        {
            for (var i = 0; i < 8; i++)
            {
                var rad = Mathf.Lerp(0.75f, 0.5f, time);
                var theta = Mathf.PI * (i * 0.25f + 0.5f);
                if (i % 2 == 0)
                {
                    theta += Mathf.PI * time * 0.25f;
                }
                else
                {
                    localRot[i] = Quaternion.Euler(0, -90, 180 * (i * 0.25f + 0.5f) + 45 * time);
                }

                localPos[i].x = rad * Mathf.Sin(Mathf.PI * 0.25f) * (i % 2 == 0 ? 1 : -1);
                localPos[i].y = rad * Mathf.Sin(theta);
                localPos[i].z = rad * Mathf.Cos(theta);
                
                var c = cube[i];
                var rot = Quaternion.Euler(0, 90, 0) * Quaternion.Euler(- 360 * time, 0, 0);
                var parentM = Matrix4x4.TRS(new Vector3(1, 1, 4), rot, Vector3.one);
                var localM  = Matrix4x4.TRS(localPos[i], localRot[i], c.Scale);
                var worldM = parentM * localM;
                c.SetPosition(worldM.GetPosition());
                c.SetRotation(worldM.rotation);
            }
        }
        private void RotCube(float time)
        {
            for (var i = 0; i < 8; i++)
            {
                var c = cube[i];
                var rot = Quaternion.Euler(0, 90, 0) * Quaternion.Euler(- 360 * time, 0, 360 * time);
                var parentM = Matrix4x4.TRS(new Vector3(1, 1, 4), rot, Vector3.one);
                var localM  = Matrix4x4.TRS(localPos[i], localRot[i], c.Scale);
                var worldM = parentM * localM;
                c.SetPosition(worldM.GetPosition());
                c.SetRotation(worldM.rotation);
            }
        }
        private void Return(float time)
        {
            for (var i = 0; i < 8; i++)
            {
                var dc = 4;
                var dt = 1f / (8 + 1 + dc);
                var t = Mathf.Clamp01((time - dt * i) / (dc * dt));
                var c = cube[i];
                if (i is 0 or 4)
                {
                    c.SetActiveTop(false);
                } else if (i is 3 or 7)
                {
                    c.SetActiveBottom(false);
                }
                else
                {
                    c.SetActiveTop(false);
                    c.SetActiveBottom(false);
                }
                var rot = Quaternion.Euler(0, 90, 0) * Quaternion.Euler(- 144 * time, 0, 144 * time);
                var parentM = Matrix4x4.TRS(new Vector3(1, 1, 4), rot, Vector3.one);
                var localM  = Matrix4x4.TRS(localPos[i], localRot[i], c.Scale);
                var worldM = parentM * localM;
                
                c.SetPosition(Vector3.Lerp(worldM.GetPosition(), initPos[i], t));
                c.SetRotation(Quaternion.Slerp(worldM.rotation, initRot[i], t));
            }
        }
    }
}