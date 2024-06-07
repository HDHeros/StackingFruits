﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameStructConfigs;
using HDH.UserData;

namespace Gameplay.LevelsLogic
{
    public class LevelsService
    {
        public IEnumerable<SectionModel> Sections;
        public int SectionsCount => _sections.Length;
        private readonly LevelsModel _dataModel;
        private readonly SectionModel[] _sections;

        
        
        public LevelsService(GameConfig gameConfig, UserDataService userDataService)
        {
            _dataModel = userDataService.GetModel<LevelsModel>();
            SectionConfig[] sectionsConfigs = gameConfig.Sections;
            _sections = new SectionModel[sectionsConfigs.Length];
            _dataModel.Sections ??= _sections;

            for (var i = 0; i < sectionsConfigs.Length; i++)
            {
                if (_dataModel.TryGetSectionById(sectionsConfigs[i].Id, out SectionModel section) == false)
                    section = new SectionModel
                    {
                        Id = sectionsConfigs[i].Id,
                        Levels = new LevelModel[sectionsConfigs[i].Levels.Length],
                    };
                section.Config = sectionsConfigs[i];
                
                if (section.Levels.Length != section.Config.Levels.Length)
                    section.Levels = new LevelModel[sectionsConfigs[i].Levels.Length]; 


                for (var j = 0; j < sectionsConfigs[i].Levels.Length; j++)
                {
                    LevelConfig levelConfig = sectionsConfigs[i].Levels[j];
                    if (section.TryGetLevel(levelConfig.Id, out LevelModel levelModel) == false)
                        levelModel = new LevelModel
                        {
                            Id = levelConfig.Id
                        };
                    levelModel.Config = levelConfig;
                    section.Levels[j] = levelModel;
                }
                
                _sections[i] = section;
            }
        }

        public void SetLevelProgress(SectionId sectionId, string levelId, float progressValue)
        {
            int sectionIndex = GetSectionIndex(sectionId);
            _sections[sectionIndex].Levels[GetLevelIndexInSection(sectionIndex, levelId)].Progress = progressValue;
            ForceSaveModel();
        }

        public bool IsLevelCompleted(SectionId sectionId, string levelId) => 
            GetLevelProgress(sectionId, levelId) >= 1;

        public float GetLevelProgress(SectionId sectionId, string levelId)
        {
            int sectionIndex = GetSectionIndex(sectionId);
            return _sections[sectionIndex].Levels[GetLevelIndexInSection(sectionIndex, levelId)].Progress;
        }

        public void SetLevelMaxScore(SectionId sectionId, string levelId, int score)
        {
            int sectionIndex = GetSectionIndex(sectionId);
            int currentScore = _sections[sectionIndex].Levels[GetLevelIndexInSection(sectionIndex, levelId)].Score;
            if (currentScore >= score) return;
            _sections[sectionIndex].Levels[GetLevelIndexInSection(sectionIndex, levelId)].Score = score;
            ForceSaveModel();
        }

        public int GetLevelScore(SectionId sectionId, string levelId)
        {
            int sectionIndex = GetSectionIndex(sectionId);
            return _sections[sectionIndex].Levels[GetLevelIndexInSection(sectionIndex, levelId)].Score;
        }

        public SectionModel GetSectionByIndex(int index) => 
            _sections[index];

        public bool IsSectionAvailable(SectionId sectionId)
        {
            for (int i = 0; i < _sections.Length; i++)
            {
                if (_sections[i].Id != sectionId) continue;
                if (i == 0) return true;
                return _sections[i - 1].IsAllLevelsCompleted;
            }

            throw new ArgumentException("No such section found");
        }

        private void ForceSaveModel()
        {
            _dataModel.Sections = _sections;
            _dataModel.ForceSave();
        }

        private int GetSectionIndex(SectionId id)
        {
            for (var i = 0; i < _sections.Length; i++)
            {
                if (_sections[i].Id != id) continue;
                return i;
            }
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        private int GetLevelIndexInSection(int sectionIndex, string levelId)
        {
            for (var i = 0; i < _sections[sectionIndex].Levels.Length; i++)
            {
                if (_sections[sectionIndex].Levels[i].Id != levelId) continue;
                return i;
            }
            
            throw new ArgumentOutOfRangeException(nameof(levelId));
        }

        [Serializable]
        public struct SectionModel
        {
            [NonSerialized] public SectionConfig Config;
            public SectionId Id;
            public LevelModel[] Levels;

            public bool IsAllLevelsCompleted => Levels.All(l => l.Progress >= 1);

            public bool TryGetLevel(string levelId, out LevelModel levelModel)
            {
                levelModel = default;
                foreach (LevelModel model in Levels)
                {
                    if (model.Id != levelId) continue;
                    levelModel = model;
                    return true;
                }

                return false;
            }
        }

        [Serializable]
        public struct LevelModel
        {
            [NonSerialized] public LevelConfig Config;
            public string Id;
            public float Progress;
            public int Score;
        }
    }
}