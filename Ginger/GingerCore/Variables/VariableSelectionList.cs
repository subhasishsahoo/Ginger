#region License
/*
Copyright © 2014-2018 European Support Limited

Licensed under the Apache License, Version 2.0 (the "License")
you may not use this file except in compliance with the License.
You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and 
limitations under the License. 
*/
#endregion

using Amdocs.Ginger.Repository;
using Amdocs.Ginger.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using GingerCore.Properties;
using GingerCore.Actions;

namespace GingerCore.Variables
{
    public class VariableSelectionList : VariableBase
    {
        public new static partial class Fields
        {
            public static string OptionalValues = "OptionalValues";
            public static string SelectedValue = "SelectedValue";
        }
               
        
        public override string VariableUIType
        {
            get { return GingerDicser.GetTermResValue(eTermResKey.Variable) + " Selection List"; }
        }

        public override string VariableEditPage { get { return "VariableSelectionListPage"; } }

        public override System.Drawing.Image Image { get { return Resources.List; } }

        public override string VariableType() { return "Selection List"; }

        //DO NOT REMOVE! Used for conversion of old OptionalValues which were kept in one string with delimiter
        public string OptionalValues
        {
            set
            {
                OptionalValuesList = ConvertOptionalValuesStringToList(value);
            }
        }
        
        [IsSerializedForLocalRepository]
        public ObservableList<OptionalValue> OptionalValuesList = new ObservableList<OptionalValue>();


        public string SelectedValue { set { Value = value; OnPropertyChanged("SelectedValue"); } get { return Value; } }


        public override string GetFormula()
        {
            string form = "Options: ";
            foreach (OptionalValue val in OptionalValuesList)
                form += val.Value + ",";
            form = form.TrimEnd(',');
            return form;
        }
 
        private ObservableList<OptionalValue> ConvertOptionalValuesStringToList(string valsString)
        {
            try
            {
                List<string> valsList = (new List<string>(valsString.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)));
                ObservableList<OptionalValue> valsObservList = new ObservableList<OptionalValue>();
                foreach (string val in valsList)
                    valsObservList.Add(new OptionalValue(val));
                return valsObservList;
            }
            catch
            {
                Reporter.ToLog(eAppReporterLogLevel.ERROR, "Cannot Convert Optional Values String To List - " + valsString);
                return new ObservableList<OptionalValue>();
            }
        }

      

        private void OptionalValuesChanged()
        {
            OnPropertyChanged("Formula");

            //make sure the selected value is valid
            if (OptionalValuesList != null && OptionalValuesList.Count > 0)
            {
                if (SelectedValue == string.Empty
                    || OptionalValuesList.Where(v => v.Value == SelectedValue).FirstOrDefault() == null)
                    SelectedValue = OptionalValuesList[0].Value;
            }
            else
                SelectedValue = string.Empty;
        }
        

        public override void ResetValue()
        {
            if (OptionalValuesList.Count > 0)
                Value = OptionalValuesList[0].Value;
        }

        public override void GenerateAutoValue()
        {
            //NA
        }

        public override bool SupportSetValue { get { return true; } }

        public override List<ActSetVariableValue.eSetValueOptions> GetSupportedOperations()
        {
            List<ActSetVariableValue.eSetValueOptions> supportedOperations = new List<ActSetVariableValue.eSetValueOptions>();
            supportedOperations.Add(ActSetVariableValue.eSetValueOptions.SetValue);
            supportedOperations.Add(ActSetVariableValue.eSetValueOptions.ResetValue);
            return supportedOperations;
        }
    }
}
