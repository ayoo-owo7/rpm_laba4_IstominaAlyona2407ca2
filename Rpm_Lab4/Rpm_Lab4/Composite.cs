using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rpm_Lab4
{
    public class Composite
    {
        public abstract class FileSystemItem
        {
            public string Name { get; set; }
            public FileSystemItem Parent { get; set; }

            protected FileSystemItem(string name, FileSystemItem parent = null)
            {
                Name = name;
                Parent = parent;
            }

            public string GetFullPath()
            {
                if (Parent == null) return "/" + Name;
                return Parent.GetFullPath() + "/" + Name;
            }

            public abstract long GetSize();
            public virtual void Add(FileSystemItem item)
            {
                throw new InvalidOperationException("Этот элемент не может иметь потомков.");
            }
            public virtual void Remove(FileSystemItem item)
            {
                throw new InvalidOperationException("Этот элемент не может удалять потомков.");
            }
            public virtual FileSystemItem GetChild(int index)
            {
                throw new InvalidOperationException("У этого элемента нет детей.");
            }

            public virtual FileSystemItem FindByName(string name)
            {
                if (this.Name == name) return this;
                return null;
            }

            public virtual List<FileSystemItem> GetChildren()
            {
                return new List<FileSystemItem>();
            }
        }

        public class FileItem : FileSystemItem
        {
            public long Size { get; private set; }
            public byte[] Content { get; private set; }

            public FileItem(string name, long size, FileSystemItem parent = null, byte[] content = null)
                : base(name, parent)
            {
                Size = size;
                Content = content ?? new byte[0];
            }

            public override long GetSize()
            {
                return Size;
            }

            public override FileSystemItem FindByName(string name)
            {
                return this.Name == name ? this : null;
            }
        }

        public class FolderItem : FileSystemItem
        {
            private List<FileSystemItem> _children = new List<FileSystemItem>();

            public FolderItem(string name, FileSystemItem parent = null) : base(name, parent) { }

            public override void Add(FileSystemItem item)
            {
                if (item == null) throw new ArgumentNullException(nameof(item));
                item.Parent = this;
                _children.Add(item);
            }

            public override void Remove(FileSystemItem item)
            {
                _children.Remove(item);
                item.Parent = null;
            }

            public override FileSystemItem GetChild(int index)
            {
                return _children[index];
            }

            public override List<FileSystemItem> GetChildren()
            {
                return new List<FileSystemItem>(_children);
            }

            public override long GetSize()
            {
                long totalSize = 0;
                foreach (var child in _children)
                {
                    totalSize += child.GetSize();
                }
                return totalSize;
            }

            public override FileSystemItem FindByName(string name)
            {
                if (this.Name == name) return this;

                foreach (var child in _children)
                {
                    var found = child.FindByName(name);
                    if (found != null) return found;
                }
                return null;
            }

            public void DeleteAll()
            {
                Console.WriteLine($"Удаление содержимого папки '{Name}'...");
                _children.Clear();
            }
        }
    }
}
