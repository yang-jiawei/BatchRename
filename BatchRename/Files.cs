using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename
{
    [Serializable]
    class Files
    {
        public struct NamePair
        {
            public string Tag;
            public string StringToFind;
        }

        private string _directory;//结尾应该有\
        private string _histroyName;
        private string _newName;
        private NamePair _nameTag;//分类用的字符串
        private string _prefix;
        private string _postfix;
        private string _dash;
        private bool _isReplace;//是否删除分类用的字符串
        private List<NamePair> _stringToReplace;//用/分割，每两个一组，用后面的替换前面的；
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

        public NamePair NameTag
        {
            get { return _nameTag; }
            set { _nameTag = value; }
        }

        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        public string Postfix
        {
            get { return _postfix; }
            set { _postfix = value; }
        }

        public string Dash
        {
            get { return _dash; }
            set { _dash = value; }
        }

        public bool IsReplace
        {
            get { return _isReplace; }
            set { _isReplace = value; }
        }

        public List<NamePair> StringToReplace
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
            if (_nameTag.Tag.Length > 0)
            {

                string name = _histroyName;
                if (_isReplace)
                {
                    name = name.Replace(_nameTag.StringToFind, "");
                }
                if (_stringToReplace.Count > 0)
                {
                    foreach (NamePair namePair in _stringToReplace)
                    {
                        name = name.Replace(namePair.StringToFind, namePair.Tag);
                    }
                }
                _newName = string.Format("{0}{1}{2}{3}{4}", _prefix, _nameTag.Tag, _postfix, _dash, name);
            }
            else
            {
                _newName = _histroyName;
            }
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
