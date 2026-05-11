using DragonBones;
using System.Collections.Generic;

namespace OBJTest.Content.NPCs.BONES
{
    public class TerrariaDBFactory : BaseFactory
    {
        public static TerrariaDBFactory Instance { get; private set; }

        private TerrariaDBFactory()
        {
            _dragonBones = new global::DragonBones.DragonBones(new NullEventDispatcher());
        }

        protected override Armature _BuildArmature(BuildArmaturePackage dataPackage)
        {
            var armature = BaseObject.BorrowObject<Armature>();
            var proxy = new TerrariaArmatureProxy();
            armature.Init(dataPackage.armature, proxy, display: null, dragonBones: _dragonBones);
            return armature;
        }

        protected override Slot _BuildSlot(BuildArmaturePackage dataPackage, SlotData slotData, Armature armature)
        {
            // DragonBones ожидает валидные raw/mesh display объекты:
            // именно их BaseFactory кладет в display-list для image/mesh.
            var rawDisplay = new object();
            var meshDisplay = new object();
            var slot = new TerrariaSlot();
            slot.Init(slotData, armature, rawDisplay, meshDisplay);
            return slot;
        }

        protected override TextureAtlasData _BuildTextureAtlasData(TextureAtlasData textureAtlasData, object textureAtlas)
        {
            if (textureAtlasData != null)
                return textureAtlasData;

            return new TerrariaTextureAtlasData();
        }
        public static void Initialize()
        {
            if (Instance == null)
            {
                Instance = new TerrariaDBFactory();
            }
        }

        private sealed class NullEventDispatcher : IEventDispatcher<EventObject>
        {
            public bool HasDBEventListener(string type) => false;
            public void DispatchDBEvent(string type, EventObject eventObject) { }
            public void AddDBEventListener(string type, ListenerDelegate<EventObject> listener) { }
            public void RemoveDBEventListener(string type, ListenerDelegate<EventObject> listener) { }
        }

        private sealed class TerrariaArmatureProxy : IArmatureProxy
        {
            private Armature _armature;
            private readonly Dictionary<string, ListenerDelegate<EventObject>> _listeners = new();

            public void DBInit(Armature armature) => _armature = armature;
            public void DBClear() => _armature = null;
            public void DBUpdate() { }

            public void Dispose(bool disposeProxy)
            {
                _armature?.Dispose();
                _armature = null;
                _listeners.Clear();
            }

            public Armature armature => _armature;
            public Animation animation => _armature?.animation;

            public bool HasDBEventListener(string type) => _listeners.ContainsKey(type);

            public void DispatchDBEvent(string type, EventObject eventObject)
            {
                if (_listeners.TryGetValue(type, out var handler) && handler != null)
                {
                    handler(type, eventObject);
                }
            }

            public void AddDBEventListener(string type, ListenerDelegate<EventObject> listener)
            {
                if (listener == null) return;
                _listeners[type] = _listeners.TryGetValue(type, out var existing) ? existing + listener : listener;
            }

            public void RemoveDBEventListener(string type, ListenerDelegate<EventObject> listener)
            {
                if (listener == null) return;
                if (_listeners.TryGetValue(type, out var existing))
                {
                    existing -= listener;
                    if (existing == null) _listeners.Remove(type);
                    else _listeners[type] = existing;
                }
            }
        }
    }
}