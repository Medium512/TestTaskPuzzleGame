using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TestTaskPuzzleGame {
    [DataContract]
    public class Level {
        [DataMember]
        public List<List<int>> template;
        [DataMember]
        public List<List<List<int>>> parts;
    }
}