using System;
using GameStructConfigs;
using HDH.UserData;
using Newtonsoft.Json;

namespace Gameplay.LevelsLogic
{
    [Serializable]
    public class LevelsModel : DataModel
    {
        [JsonProperty] public LevelsService.SectionModel[] Sections;

        public bool TryGetSectionById(SectionId id, out LevelsService.SectionModel sectionModel)
        {
            sectionModel = default;
            foreach (var section in Sections)
            {
                if (section.Id != id) continue;
                sectionModel = section;
                return true;
            }

            return false;
        }
    }
}