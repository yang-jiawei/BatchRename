using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename
{
    [Serializable]
    class File
    {
        private string _directory;//结尾应该有\
        private string _histroyName;
        private string _newName;
        private string _stringToFind;//分类用的字符串
        private string _category;//分类
        private char _prefix;
        private char _postfix;
        private char _dash;
        private bool _isReplace;//是否删除分类用的字符串
        private string _stringToReplace;//用/分割，每两个一组，用后面的替换前面的；
        private bool _status;

        public string Directory
        {
            get { return _directory; }
            set { _directory = value; }
        }

        public string HistoryName
        {
            get { return _histroyName; }
            set { _histroyName = value; }
        }

        public string NewName
        {
            get { return _newName; }
            set { _newName = value; }
        }

        public string StringToFind
        {
            get { return _stringToFind; }
            set { _stringToFind = value; }
        }

        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }

        public char Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        public char Postfix
        {
            get { return _postfix; }
            set { _postfix = value; }
        }

        public char Dash
        {
            get { return _dash; }
            set { _dash = value; }
        }

        public bool IsReplace
        {
            get { return _isReplace; }
            set { _isReplace = value; }
        }

        public string StringToReplace
        {
            get { return _stringToReplace; }
            set { _stringToReplace = value; }
        }

        public bool Status
        {
            get { return _status; }
            set { _status = value; }

        }

        public void GetNewName()
        {
            string name = _histroyName;
            if (_isReplace)
            {
                name = name.Replace(_stringToFind, "");
            }
            if (_stringToReplace.Length > 0 && _stringToReplace.Length%2 == 0)
            {
                string[] replaceStrings = _stringToReplace.Split('/');
                for (int index = 0; index < _stringToReplace.Length/2; index++)
                {
                    name = name.Replace(replaceStrings[index*2], replaceStrings[index*2 + 1]);
                }
            }
            _newName = string.Format("{0},{1},{2},{3},{4}", _prefix, _category, _postfix, _dash, name);
        }

        public bool Rename()
        {
            if (_histroyName == _newName) return true;
            try
            {
                System.IO.File.Move(_directory + _histroyName, _directory + _newName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
