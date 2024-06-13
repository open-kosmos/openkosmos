using System;
using Kosmos.Time;
using Unity.Collections;
using Unity.Entities;

namespace Kosmos.UI.Debug
{
    public partial class DebugTimeUiUpdateSystem : SystemBase
    {
        private const string DATE_FORMAT = "dd MMMM yyyy";
        private const string TIME_FORMAT = "HH:mm:ss.ss";
        
        protected override void OnCreate()
        {
            RequireForUpdate<DebugTimeUI>();
            RequireForUpdate<IsCurrentPlayerTimelineTag>();
        }

        protected override void OnUpdate()
        {
            var currentUniversalTime = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UniversalTime, IsCurrentPlayerTimelineTag>()
                .Build(this);

            var timeEntity = currentUniversalTime.GetSingletonEntity();
            
            var universalTime = EntityManager.GetComponentData<UniversalTime>(timeEntity);
            
            //TODO: This should be data-driven from some sort of scenario file.
            var referenceDate = new DateTime(2070, 1, 1, 0, 0, 0);

            var date = referenceDate.AddSeconds(universalTime.Value);
            
            Entities
                .ForEach((DebugTimeUI debugTimeUi) =>
                {
                    debugTimeUi.SetText($"{date.ToString(DATE_FORMAT)}\n{date.ToString(TIME_FORMAT)}");
                })
            .WithoutBurst()
            .Run();
        }
    }
}
