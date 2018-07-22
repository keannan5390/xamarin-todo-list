﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MonkeyCache;
using TodoApp.Models;

namespace TodoApp.Repositories
{
	public interface ITodoRepository
	{
		Task<IEnumerable<TodoList>> GetLists();

		Task<TodoList> AddList(TodoList list);

		Task UpdateList(Guid id, TodoList list);

		Task RemoveList(Guid id);

		Task DeactivateList(Guid listId);

		Task ActivateList(Guid listId);

		Task<IEnumerable<ToDoItem>> GetItems(Guid listId);

		Task<ToDoItem> AddItem(Guid listId, ToDoItem item);

		Task RemoveItem(Guid listId, Guid itemId);
	}

	public class TodoRepository : BaseRepository, ITodoRepository
	{
		const string LIST_KEY = "todoLists";

		public TodoRepository(IBarrel barrel)
			: base(barrel)
		{
		}

		public Task<TodoList> GetList(Guid listId) => Get<TodoList>(LIST_KEY, listId);

		public Task<IEnumerable<TodoList>> GetLists() => Get<TodoList>(LIST_KEY);

		public Task<TodoList> AddList(TodoList newList) => base.Create(LIST_KEY, newList);

		public Task UpdateList(Guid id, TodoList list) => base.Update(LIST_KEY, id, list);

		public Task RemoveList(Guid id) => base.Delete<TodoList>(LIST_KEY, id);

		public Task<IEnumerable<ToDoItem>> GetItems(Guid listId) => base.Get<ToDoItem>(KeyForItem(listId));

		public Task<ToDoItem> AddItem(Guid listId, ToDoItem item) => base.Create<ToDoItem>(KeyForItem(listId), item);

		public Task RemoveItem(Guid listId, Guid itemId) => base.Delete<ToDoItem>(KeyForItem(listId), itemId);

		public async Task DeactivateList(Guid listId)
		{
			var allItems = (await GetLists()).ToList();

			foreach(var item in allItems)
			{
				// just deactivate all lists for simplicity right now
				item.IsActive = false;
			}

			await this.Save(LIST_KEY, allItems);
		}

		public async Task ActivateList(Guid listId)
		{
			var allItems = await GetLists();

			foreach (var item in allItems)
			{
				// active the the target list
				item.IsActive = item.Id == listId;
			}

			await this.Save(LIST_KEY, allItems);
		}


		string KeyForItem(Guid listId)
		{
			string listKey = listId.ToString().Replace("-", string.Empty);
			return $"ToDoItem{listKey}";
		}
	}
}
