﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utilities
{
	public interface IUndoRedo
	{
		string Name { get; }
		void Undo();
		void Redo();
	}

	public class UndoRedoAction : IUndoRedo
	{
		private Action _undoAction;
		private Action _redoAction;

		public string Name { get; }

		public void Redo() => _redoAction();

		public void Undo() => _undoAction();

		public UndoRedoAction(string name)
		{
			Name = name;
		}

		public UndoRedoAction(Action undo, Action redo, string name)
		{
			Name = name;

			Debug.Assert(undo != null && redo != null);
			_undoAction = undo;
			_redoAction = redo;
		}
	}

	public class UndoRedo
	{
		private readonly ObservableCollection<IUndoRedo> _redoList = new ObservableCollection<IUndoRedo>();
		private readonly ObservableCollection<IUndoRedo> _undoList = new ObservableCollection<IUndoRedo>();

		public ReadOnlyObservableCollection<IUndoRedo> RedoList { get; }
		public ReadOnlyObservableCollection<IUndoRedo> UndoList { get; }

		public UndoRedo()
		{
			RedoList = new ReadOnlyObservableCollection<IUndoRedo>(_redoList);
			UndoList = new ReadOnlyObservableCollection<IUndoRedo>(_undoList);
		}

		public void Undo()
		{
			if (_undoList.Any())
			{
				IUndoRedo cmd = _undoList.Last();
				_undoList.RemoveAt(_undoList.Count - 1);
				cmd.Undo();
				_redoList.Insert(0, cmd);
			}
		}

		public void Redo()
		{
			if (_redoList.Any())
			{
				IUndoRedo cmd = _redoList.First();
				_redoList.RemoveAt(0);
				cmd.Redo();
				_undoList.Add(cmd);
			}
		}

		public void Add(IUndoRedo cmd)
		{
			_undoList.Add(cmd);
			_redoList.Clear();
		}

		public void Reset()
		{
			_redoList.Clear();
			_undoList.Clear();
		}
	}
}