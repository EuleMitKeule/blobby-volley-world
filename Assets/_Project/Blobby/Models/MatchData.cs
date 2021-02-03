using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blobby.Models
{
    [Serializable]
    public class MatchData
    {
        public Map Map = Map.Gym;
        public GameMode GameMode = GameMode.Standard;
        public PlayerMode PlayerMode = PlayerMode.Single;
        public JumpMode JumpMode = JumpMode.Standard;
        public bool JumpOverNet = false;
        public float TimeScale = 1;

        public float HitTreshold { get { return 0.175f; } }

        public int WinningScore 
        { 
            get 
            {
                if (GameMode == GameMode.Blitz) return 2;
                else return 16;
            } 
        }

        public int PlayerCount
        {
            get
            {
                return (PlayerMode == PlayerMode.Single || PlayerMode == PlayerMode.Ghost) ? 2 : 4;
            }
        }

        public Vector2[] SpawnPoints
        {
            get
            {
                if (PlayerCount == 2)
                {
                    return new Vector2[] { new Vector2(-10, Ground), new Vector2(10, Ground) };
                }
                else
                {
                    return new Vector2[]
                    {
                        new Vector2(-12, Ground),
                        new Vector2(12, Ground),
                        new Vector2(-8, Ground),
                        new Vector2(8, Ground)
                    };
                }
            }
        }

        public float[] LeftLimits
        {
            get
            {
                if (!JumpOverNet)
                {
                    if (PlayerCount == 2)
                    {
                        return new float[] { -19.25f, 1.75f };
                    }
                    else if (PlayerMode == PlayerMode.Double)
                    {
                        return new float[] { -19.25f, 1.75f, -19.25f, 1.75f };
                    }
                    else
                    {
                        return new float[] { -19.25f, 10f, -10f, 1.75f };
                    }
                }
                else
                {
                    if (PlayerCount == 2)
                    {
                        return new float[] { -19.25f, -19.25f };
                    }
                    else if (PlayerMode == PlayerMode.Double)
                    {
                        return new float[] { -19.25f, -19.25f, -19.25f, -19.25f };
                    }
                    else
                    {
                        return new float[] { -19.25f, 10f, -10f, -10f };
                    }
                }
            }
        }

        public float[] RightLimits
        {
            get
            {
                if (!JumpOverNet)
                {
                    if (PlayerCount == 2)
                    {
                        return new float[] { -1.75f, 19.25f };
                    }
                    else if (PlayerMode == PlayerMode.Double)
                    {
                        return new float[] { -1.75f, 19.25f, -1.75f, 19.25f };
                    }
                    else
                    {
                        return new float[] { -10f, 19.25f, -1.75f, 10f };
                    }
                }
                else
                {
                    if (PlayerCount == 2)
                    {
                        return new float[] { 19.25f, 19.25f };
                    }
                    else if (PlayerMode == PlayerMode.Double)
                    {
                        return new float[] { 19.25f, 19.25f, 19.25f, 19.25f };
                    }
                    else
                    {
                        return new float[] { -10f, 19.25f, 10f, 10f };
                    }
                }
            }
        }

        public int[] AllowedHits
        {
            get
            {
                if (GameMode == GameMode.Tennis)
                {
                    return new int[] { 1, 1, 1, 1, 1, 1 };
                }
                else
                {
                    if (PlayerCount == 2)
                    {
                        return new int[] { 3, 3, 3, 3, 3, 3 };
                    }
                    else
                    {
                        return new int[] { 1, 1, 1, 1, 3, 3 };
                    }
                }
            }
        }

        public GameObject BallPrefab
        {
            get
            {
                if (GameMode == GameMode.Bomb)
                    return Resources.Load<GameObject>("Prefabs/ball/bomb");
                else
                    return Resources.Load<GameObject>("Prefabs/ball/ball");
            }
        }

        public float Ground { get { return -6.75f; } }

        public float BallGround { get { return -7.35f; } }

        public float[] PlayerColliderOffsets
        {
            get
            {
                return new float[] { 0.97f, -0.75f };
            }
        }

        public float[] PlayerColliderRadius
        {
            get
            {
                return new float[] { 1.14f, 1.54f };
            }
        }

        public float PlayerColliderCenterOffset { get { return -0.3f; } }



        public float[] NetPositions
        {
            get
            {
                return new float[] { -1.75f, 1.75f };
            }
        }

        public float ArrowHeight { get { return 10.5f; } }

        public float ArrowLimit { get { return 10.85f; } }

    }
}
