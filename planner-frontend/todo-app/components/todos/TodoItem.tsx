'use client';

import { useState } from 'react';
import { useUpdateTodoMutation, useDeleteTodoMutation } from '@/lib/api/todosApi';
import { Todo, TodoStatus, Priority } from '@/lib/types';
import SubtaskList from './SubtaskList';
import { CheckIcon, Bars3Icon } from '@heroicons/react/24/outline';
import { useToast } from '@/components/ui/Toast';
import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';

interface TodoItemProps {
  todo: Todo;
}

const statusColors = {
  [TodoStatus.Pending]: 'bg-gray-100 text-gray-800',
  [TodoStatus.InProgress]: 'bg-blue-100 text-blue-800',
  [TodoStatus.Completed]: 'bg-green-100 text-green-800',
  [TodoStatus.Cancelled]: 'bg-red-100 text-red-800',
};

const priorityColors = {
  [Priority.Low]: 'bg-gray-100 text-gray-800',
  [Priority.Medium]: 'bg-yellow-100 text-yellow-800',
  [Priority.High]: 'bg-orange-100 text-orange-800',
  [Priority.Critical]: 'bg-red-100 text-red-800',
};

export default function TodoItem({ todo }: TodoItemProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [showSubtasks, setShowSubtasks] = useState(false);
  const { addToast } = useToast();
  
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: todo.id.toString() });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  };
  const [editData, setEditData] = useState({
    title: todo.title,
    description: todo.description,
    status: todo.status,
    priority: todo.priority,
    dueDate: todo.dueDate || '',
  });

  const [updateTodo, { isLoading: isUpdating }] = useUpdateTodoMutation();
  const [deleteTodo, { isLoading: isDeleting }] = useDeleteTodoMutation();

  const handleSave = async () => {
    try {
      await updateTodo({
        id: todo.id,
        todo: {
          ...editData,
          dueDate: editData.dueDate || null,
        },
      }).unwrap();
      setIsEditing(false);
      addToast({
        type: 'success',
        title: 'Todo updated successfully',
      });
    } catch (error) {
      console.error('Failed to update todo:', error);
      addToast({
        type: 'error',
        title: 'Failed to update todo',
        message: 'Please try again',
      });
    }
  };

  const handleDelete = async () => {
    if (confirm('Are you sure you want to delete this todo?')) {
      try {
        await deleteTodo(todo.id).unwrap();
        addToast({
          type: 'success',
          title: 'Todo deleted successfully',
        });
      } catch (error) {
        console.error('Failed to delete todo:', error);
        addToast({
          type: 'error',
          title: 'Failed to delete todo',
          message: 'Please try again',
        });
      }
    }
  };

  const toggleComplete = async () => {
    try {
      const newStatus = todo.status === TodoStatus.Completed ? TodoStatus.Pending : TodoStatus.Completed;
      await updateTodo({
        id: todo.id,
        todo: {
          title: todo.title,
          description: todo.description,
          status: newStatus,
          priority: todo.priority,
          dueDate: todo.dueDate,
        },
      }).unwrap();
    } catch (error) {
      console.error('Failed to toggle todo completion:', error);
      addToast({
        type: 'error',
        title: 'Failed to update todo',
        message: 'Please try again',
      });
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
  };

  if (isEditing) {
    return (
      <div 
        ref={setNodeRef} 
        style={style}
        className={`card p-6 ${isDragging ? 'opacity-50 shadow-2xl' : ''}`}
      >
        <div className="space-y-4">
          <input
            type="text"
            value={editData.title}
            onChange={(e) => setEditData(prev => ({ ...prev, title: e.target.value }))}
            className="input text-lg font-medium"
            placeholder="Todo title"
          />
          
          <textarea
            value={editData.description}
            onChange={(e) => setEditData(prev => ({ ...prev, description: e.target.value }))}
            className="input min-h-[80px] resize-none"
            placeholder="Todo description"
          />
          
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <select
              value={editData.status}
              onChange={(e) => setEditData(prev => ({ ...prev, status: parseInt(e.target.value) as TodoStatus }))}
              className="input"
            >
              <option value={TodoStatus.Pending}>Pending</option>
              <option value={TodoStatus.InProgress}>In Progress</option>
              <option value={TodoStatus.Completed}>Completed</option>
              <option value={TodoStatus.Cancelled}>Cancelled</option>
            </select>
            
            <select
              value={editData.priority}
              onChange={(e) => setEditData(prev => ({ ...prev, priority: parseInt(e.target.value) as Priority }))}
              className="input"
            >
              <option value={Priority.Low}>Low</option>
              <option value={Priority.Medium}>Medium</option>
              <option value={Priority.High}>High</option>
              <option value={Priority.Critical}>Critical</option>
            </select>
            
            <input
              type="date"
              value={editData.dueDate}
              onChange={(e) => setEditData(prev => ({ ...prev, dueDate: e.target.value }))}
              className="input"
            />
          </div>
          
          <div className="flex gap-2">
            <button
              onClick={handleSave}
              disabled={isUpdating}
              className="btn-primary"
            >
              {isUpdating ? 'Saving...' : 'Save'}
            </button>
            <button
              onClick={() => setIsEditing(false)}
              className="btn-secondary"
            >
              Cancel
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div 
      ref={setNodeRef} 
      style={style}
      className={`card p-6 ${isDragging ? 'opacity-50 shadow-2xl rotate-3 scale-105' : ''}`}
    >
      <div className="flex items-start justify-between mb-4">
        <div className="flex items-start gap-3 flex-1">
          <button
            {...attributes}
            {...listeners}
            className="flex-shrink-0 p-1 hover:bg-gray-100 rounded cursor-grab active:cursor-grabbing transition-colors"
            aria-label="Drag to reorder"
          >
            <Bars3Icon className="w-5 h-5 text-gray-400" />
          </button>
          <button
            onClick={toggleComplete}
            className={`checkbox ${
              todo.status === TodoStatus.Completed
                ? 'checkbox-completed'
                : 'checkbox-pending'
            }`}
          >
            {todo.status === TodoStatus.Completed && (
              <CheckIcon className="w-4 h-4" />
            )}
          </button>
          <div className="flex-1">
            <h3 className={`text-lg font-medium mb-2 ${
              todo.status === TodoStatus.Completed
                ? 'text-gray-500 line-through'
                : 'text-gray-900'
            }`}>
              {todo.title}
            </h3>
            <p className={`mb-3 ${
              todo.status === TodoStatus.Completed
                ? 'text-gray-400 line-through'
                : 'text-gray-600'
            }`}>
              {todo.description}
            </p>
            
            <div className="flex flex-wrap items-center gap-2 mb-3">
              <span className={`px-2 py-1 rounded-full text-xs font-medium ${statusColors[todo.status]}`}>
                {TodoStatus[todo.status]}
              </span>
              <span className={`px-2 py-1 rounded-full text-xs font-medium ${priorityColors[todo.priority]}`}>
                {Priority[todo.priority]} Priority
              </span>
              {todo.dueDate && (
                <span className="px-2 py-1 bg-gray-100 text-gray-800 rounded-full text-xs font-medium">
                  Due: {formatDate(todo.dueDate)}
                </span>
              )}
            </div>
            
            <div className="text-xs text-gray-500">
              Created: {formatDate(todo.createdAt)} • Updated: {formatDate(todo.updatedAt)}
            </div>
          </div>
        </div>
        
        <div className="flex gap-2 ml-4">
          <button
            onClick={() => setIsEditing(true)}
            className="text-blue-600 hover:text-blue-800 text-sm font-medium"
          >
            Edit
          </button>
          <button
            onClick={handleDelete}
            disabled={isDeleting}
            className="text-red-600 hover:text-red-800 text-sm font-medium disabled:opacity-50"
          >
            {isDeleting ? 'Deleting...' : 'Delete'}
          </button>
        </div>
      </div>
      
      {todo.subtasks && todo.subtasks.length > 0 && (
        <div className="border-t pt-4">
          <button
            onClick={() => setShowSubtasks(!showSubtasks)}
            className="flex items-center gap-2 text-sm font-medium text-gray-700 hover:text-gray-900 mb-3"
          >
            <span>{showSubtasks ? '▼' : '▶'}</span>
            Subtasks ({todo.subtasks.length})
          </button>
          
          {showSubtasks && (
            <SubtaskList 
              todoId={todo.id} 
              subtasks={todo.subtasks} 
              parentTodo={{
                id: todo.id,
                title: todo.title,
                description: todo.description,
                priority: todo.priority,
                dueDate: todo.dueDate
              }}
            />
          )}
        </div>
      )}
      
      {(!todo.subtasks || todo.subtasks.length === 0) && (
        <div className="border-t pt-4">
          <SubtaskList 
            todoId={todo.id} 
            subtasks={[]} 
            parentTodo={{
              id: todo.id,
              title: todo.title,
              description: todo.description,
              priority: todo.priority,
              dueDate: todo.dueDate
            }}
          />
        </div>
      )}
    </div>
  );
}