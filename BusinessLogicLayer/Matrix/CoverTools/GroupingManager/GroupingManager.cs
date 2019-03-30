using System.Collections.Generic;
using Core.Common.Interfaces.GroupingManager;

namespace BusinessLogicLayer.Matrix.CoverTools.GroupingManager
{
    public class GroupingManager : IGroupingManager
    {
        private readonly IGroupingMethod[] _groupingMethods;

        public GroupingManager(IGroupingMethod[] groupingMethods)
        {
            _groupingMethods = groupingMethods;
        }
        
        public IEnumerable<IGroupingMethod> GetGroupingMethods()
        {
            return _groupingMethods;
        }
    }
}
