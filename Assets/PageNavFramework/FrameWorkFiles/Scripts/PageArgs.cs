using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace PageNavFrameWork{
	[System.Serializable]
	public class PageArgs{

		#if UNITY_EDITOR 
		[HideInInspector]
		public bool _expandedEditor = false;
		public bool _expandedEditorEntireProperty = false;
		public string displayName = null;
		#endif

		public PagesEnum PageToCreate;

		public enum PageArgType{objectType,intType,stringType,boolType};

		[System.Serializable]
		public class ArgItemHolder{
			public string name = null;
			public PageArgType type = PageArgType.objectType;
			public UnityEngine.Object objectValue = null;
			public int intValue;
			public string stringValue = null;
			public bool boolValue;
            public ArgItemHolder() {}
            public ArgItemHolder(string name, UnityEngine.Object value)
            {
                this.name = name;
                this.objectValue = value;
                this.type = PageArgType.objectType;
            }
            public ArgItemHolder(string name, int value)
            {
                this.name = name;
                this.intValue = value;
                this.type = PageArgType.intType;
            }
            public ArgItemHolder(string name, string value)
            {
                this.name = name;
                this.stringValue = value;
                this.type = PageArgType.stringType;
            }
            public ArgItemHolder(string name, bool value)
            {
                this.name = name;
                this.boolValue = value;
                this.type = PageArgType.boolType;
            }

        }

		public List<ArgItemHolder> ArgumentList = new List<ArgItemHolder> ();

        public void addArg(String name, UnityEngine.Object value)
        {
            TestForNewArgName(name);
            ArgItemHolder item = new ArgItemHolder(name,value);
            ArgumentList.Add(item);
        }

        public void addArg(String name, int value)
        {
            TestForNewArgName(name);
            ArgItemHolder item = new ArgItemHolder(name, value);
            ArgumentList.Add(item);
        }

        public void addArg(String name, string value)
        {
            TestForNewArgName(name);
            ArgItemHolder item = new ArgItemHolder(name, value);
            ArgumentList.Add(item);
        }

        public void addArg(String name, bool value)
        {
            TestForNewArgName(name);
            ArgItemHolder item = new ArgItemHolder(name, value);
            ArgumentList.Add(item);
        }

        public void removeArg(String name)
        {
            ArgItemHolder itemToRemove = null;
            foreach (ArgItemHolder arg in ArgumentList)
            {
                if (arg.name == name)
                {
                    itemToRemove = arg;
                    break;
                }
            }
            if (itemToRemove != null)
            {
                ArgumentList.Remove(itemToRemove);
            }
        }

        private void TestForNewArgName(string name)
        {
            foreach (ArgItemHolder arg in ArgumentList)
            {
                if (arg.name == name)
                {
                    throw new UnityException("Two arguments ('" + arg.name + "') can't have the same name!");
                }
            }
        }

        [HideInInspector]
		private Dictionary<string,object> _dictionaryArgs = new Dictionary<string, object> ();
		public Dictionary<string,object> DictionaryArgs{
			get{
				if (_dictionaryArgs.Count == 0) {
					RegenerateDictionary ();
				}
				return _dictionaryArgs;
			}
			set{
				_dictionaryArgs = value;
			}
		}



        public void RegenerateDictionary(){
			_dictionaryArgs.Clear ();
			foreach(ArgItemHolder arg in ArgumentList){
				if(_dictionaryArgs.ContainsKey(arg.name)){
					throw new UnityException ("Two arguments ('" + arg.name + "') can't have the same name!");
				}
				switch(arg.type){
				case PageArgType.objectType:
					_dictionaryArgs.Add (arg.name,arg.objectValue);
					break;
				case PageArgType.intType:
					_dictionaryArgs.Add (arg.name,arg.intValue);
					break;
				case PageArgType.stringType:
					_dictionaryArgs.Add (arg.name,arg.stringValue);
					break;
				case PageArgType.boolType:
					_dictionaryArgs.Add (arg.name,arg.boolValue);
					break;
				}
			}
		}
	}
}