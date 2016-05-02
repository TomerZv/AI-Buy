using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public sealed class BusinessLogicGateway
    {
        #region Singleton
        private static volatile BusinessLogicGateway instance;
        private static object syncRoot = new Object();
        public static BusinessLogicGateway Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BusinessLogicGateway();
                    }
                }

                return instance;
            }
        } 
        #endregion

        #region Data Members

        public List<> MyProperty { get; set; }

        #endregion

        public void ReadAuctions()
        {

        }
    }
}
