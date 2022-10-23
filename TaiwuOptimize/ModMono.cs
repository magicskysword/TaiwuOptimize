using System;
using System.Collections.Generic;
using System.Diagnostics;
using TaiwuOptimize.Behaviour;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TaiwuOptimize
{
    public class ModMono : MonoBehaviour
    {
        private Queue<AvatarRecord> _avatarRefreshQueue = new Queue<AvatarRecord>();
        private HashSet<AvatarRecord> _avatarRefreshSet = new HashSet<AvatarRecord>();
        private float targetFrame = 60f;
        private Stopwatch _stopwatch = new Stopwatch();

        private void Awake()
        {
            var rendererGo = new GameObject("AvatarRenderer");
            rendererGo.transform.SetParent(transform);
        }

        private void Update()
        {
            _stopwatch.Restart();
            long maxDeltaTime = (long)(1000f / targetFrame);
            while (_avatarRefreshQueue.Count > 0)
            {
                var avatarRecord = DequeueRefreshAvatar();
                if (avatarRecord != null)
                {
                    if (avatarRecord.IsActive())
                    {
                        avatarRecord.Refresh();
                    }
                    else
                    {
                        avatarRecord.NeedRefresh = true;
                    }
                }
                if(_stopwatch.ElapsedMilliseconds > maxDeltaTime)
                    break;
            }
        }

        public void EnqueueRefreshAvatar(AvatarRecord record)
        {
            if(_avatarRefreshSet.Contains(record))
                return;
            _avatarRefreshQueue.Enqueue(record);
            _avatarRefreshSet.Add(record);
        }
        
        public AvatarRecord DequeueRefreshAvatar()
        {
            var record = _avatarRefreshQueue.Dequeue();
            _avatarRefreshSet.Remove(record);
            return record;
        }
    }
}