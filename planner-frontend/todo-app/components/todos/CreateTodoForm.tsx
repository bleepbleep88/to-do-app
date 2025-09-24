'use client';

import { useState } from 'react';
import { useCreateTodoMutation } from '@/lib/api/todosApi';
import { Priority, type CreateTodoRequest } from '@/lib/types';

export default function CreateTodoForm() {
  const [isOpen, setIsOpen] = useState(false);
  const [formData, setFormData] = useState<CreateTodoRequest>({
    title: '',
    description: '',
    priority: Priority.Medium,
    dueDate: null,
  });

  const [createTodo, { isLoading }] = useCreateTodoMutation();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    try {
      await createTodo({
        ...formData,
        dueDate: formData.dueDate || null,
      }).unwrap();
      
      // Reset form
      setFormData({
        title: '',
        description: '',
        priority: Priority.Medium,
        dueDate: null,
      });
      setIsOpen(false);
    } catch (error) {
      console.error('Failed to create todo:', error);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'priority' ? parseInt(value) as Priority : value,
    }));
  };

  if (!isOpen) {
    return (
      <div className="card p-4">
        <button
          onClick={() => setIsOpen(true)}
          className="w-full p-4 text-left text-gray-500 hover:text-gray-700 hover:bg-gray-50 rounded-lg border-2 border-dashed border-gray-200 hover:border-gray-300 transition-colors"
        >
          + Add a new todo
        </button>
      </div>
    );
  }

  return (
    <div className="card p-6">
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <input
            type="text"
            name="title"
            value={formData.title}
            onChange={handleChange}
            className="input text-lg font-medium"
            placeholder="What needs to be done?"
            required
            autoFocus
          />
        </div>

        <div>
          <textarea
            name="description"
            value={formData.description}
            onChange={handleChange}
            className="input min-h-[80px] resize-none"
            placeholder="Add a description (optional)"
          />
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Priority
            </label>
            <select
              name="priority"
              value={formData.priority}
              onChange={handleChange}
              className="input"
            >
              <option value={Priority.Low}>Low</option>
              <option value={Priority.Medium}>Medium</option>
              <option value={Priority.High}>High</option>
              <option value={Priority.Critical}>Critical</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Due Date (optional)
            </label>
            <input
              type="date"
              name="dueDate"
              value={formData.dueDate || ''}
              onChange={handleChange}
              className="input"
            />
          </div>
        </div>

        <div className="flex gap-2">
          <button
            type="submit"
            disabled={isLoading || !formData.title.trim()}
            className="btn-primary disabled:opacity-50"
          >
            {isLoading ? 'Creating...' : 'Create Todo'}
          </button>
          <button
            type="button"
            onClick={() => setIsOpen(false)}
            className="btn-secondary"
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}