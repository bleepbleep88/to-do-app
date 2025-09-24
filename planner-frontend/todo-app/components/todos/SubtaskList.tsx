'use client';

import { useState, Dispatch, SetStateAction } from 'react';
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  DragEndEvent,
} from '@dnd-kit/core';
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  verticalListSortingStrategy,
  useSortable,
} from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { 
  useCreateSubtaskMutation, 
  useUpdateSubtaskMutation, 
  useDeleteSubtaskMutation,
  useUpdateTodoMutation,
  useReorderSubtasksMutation
} from '@/lib/api/todosApi';
import { Subtask, TodoStatus, type CreateSubtaskRequest, type UpdateSubtaskRequest } from '@/lib/types';
import { CheckIcon, Bars3Icon } from '@heroicons/react/24/outline';
import { useToast } from '@/components/ui/Toast';

type EditDataType = UpdateSubtaskRequest & { id: number };

interface SubtaskListProps {
  todoId: number;
  subtasks: Subtask[];
  parentTodo?: {
    id: number;
    title: string;
    description: string;
    priority: number;
    dueDate: string | null;
  };
}

export default function SubtaskList({ todoId, subtasks, parentTodo }: SubtaskListProps) {
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const { addToast } = useToast();
  
  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );
  const [formData, setFormData] = useState<CreateSubtaskRequest>({
    title: '',
    description: '',
    order: subtasks.length,
  });
  const [editData, setEditData] = useState<EditDataType>({
    id: 0,
    title: '',
    description: '',
    status: TodoStatus.Pending,
    order: 0,
  });
  
  const handleEditDataChange = (updater: (prev: EditDataType) => EditDataType) => {
    setEditData(updater);
  };

  const [createSubtask, { isLoading: isCreating }] = useCreateSubtaskMutation();
  const [updateSubtask, { isLoading: isUpdating }] = useUpdateSubtaskMutation();
  const [deleteSubtask, { isLoading: isDeleting }] = useDeleteSubtaskMutation();
  const [updateTodo] = useUpdateTodoMutation();
  const [reorderSubtasks] = useReorderSubtasksMutation();

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    
    try {
      await createSubtask({
        todoId,
        subtask: formData,
      }).unwrap();
      
      setFormData({
        title: '',
        description: '',
        order: subtasks.length + 1,
      });
      setShowCreateForm(false);
    } catch (error) {
      console.error('Failed to create subtask:', error);
    }
  };

  const handleEdit = (subtask: Subtask) => {
    setEditData({
      id: subtask.id,
      title: subtask.title,
      description: subtask.description,
      status: subtask.status,
      order: subtask.order,
    });
    setEditingId(subtask.id);
  };

  const handleUpdate = async () => {
    try {
      await updateSubtask({
        todoId,
        id: editData.id,
        subtask: {
          title: editData.title,
          description: editData.description,
          status: editData.status,
          order: editData.order,
        },
      }).unwrap();
      setEditingId(null);
    } catch (error) {
      console.error('Failed to update subtask:', error);
    }
  };

  const handleDelete = async (id: number) => {
    if (confirm('Are you sure you want to delete this subtask?')) {
      try {
        await deleteSubtask({ todoId, id }).unwrap();
      } catch (error) {
        console.error('Failed to delete subtask:', error);
      }
    }
  };

  const toggleSubtaskComplete = async (subtask: Subtask) => {
    try {
      const newStatus = subtask.status === TodoStatus.Completed ? TodoStatus.Pending : TodoStatus.Completed;
      
      await updateSubtask({
        todoId,
        id: subtask.id,
        subtask: {
          title: subtask.title,
          description: subtask.description,
          status: newStatus,
          order: subtask.order,
        },
      }).unwrap();

      // Check if all subtasks are completed and update parent todo if needed
      if (newStatus === TodoStatus.Completed && parentTodo) {
        const updatedSubtasks = subtasks.map(s => 
          s.id === subtask.id ? { ...s, status: newStatus } : s
        );
        const allCompleted = updatedSubtasks.every(s => s.status === TodoStatus.Completed);
        
        if (allCompleted) {
          await updateTodo({
            id: parentTodo.id,
            todo: {
              title: parentTodo.title,
              description: parentTodo.description,
              status: TodoStatus.Completed,
              priority: parentTodo.priority,
              dueDate: parentTodo.dueDate,
            },
          }).unwrap();
        }
      }
    } catch (error) {
      console.error('Failed to toggle subtask completion:', error);
    }
  };

  const handleDragEnd = async (event: DragEndEvent) => {
    const { active, over } = event;

    if (active.id !== over?.id && subtasks) {
      const oldIndex = subtasks.findIndex((subtask) => subtask.id.toString() === active.id);
      const newIndex = subtasks.findIndex((subtask) => subtask.id.toString() === over?.id);

      const reorderedSubtasks = arrayMove(subtasks, oldIndex, newIndex);
      
      // Send the new order to the backend
      try {
        await reorderSubtasks({
          todoId,
          subtaskIds: reorderedSubtasks.map(subtask => subtask.id)
        }).unwrap();
      } catch (error) {
        console.error('Failed to reorder subtasks:', error);
        addToast({
          type: 'error',
          title: 'Failed to reorder subtasks',
          message: 'Please try again',
        });
      }
    }
  };

  const statusColors = {
    [TodoStatus.Pending]: 'bg-gray-100 text-gray-800',
    [TodoStatus.InProgress]: 'bg-blue-100 text-blue-800',
    [TodoStatus.Completed]: 'bg-green-100 text-green-800',
    [TodoStatus.Cancelled]: 'bg-red-100 text-red-800',
  };

  return (
    <div className="space-y-3">
      {subtasks.length > 0 ? (
        <DndContext 
          sensors={sensors}
          collisionDetection={closestCenter}
          onDragEnd={handleDragEnd}
        >
          <SortableContext 
            items={subtasks.map(subtask => subtask.id.toString())}
            strategy={verticalListSortingStrategy}
          >
            {subtasks.map((subtask) => (
              <SortableSubtaskItem
                key={subtask.id}
                subtask={subtask}
                editingId={editingId}
                editData={editData}
                isUpdating={isUpdating}
                isDeleting={isDeleting}
                statusColors={statusColors}
                onEdit={handleEdit}
                onUpdate={handleUpdate}
                onDelete={handleDelete}
                onToggleComplete={toggleSubtaskComplete}
                setEditingId={setEditingId}
                setEditData={setEditData}
                handleEditDataChange={handleEditDataChange}
              />
            ))}
          </SortableContext>
        </DndContext>
      ) : null}

      {showCreateForm ? (
        <div className="bg-gray-50 rounded-lg p-4">
          <form onSubmit={handleCreate} className="space-y-3">
            <input
              type="text"
              value={formData.title}
              onChange={(e) => setFormData(prev => ({ ...prev, title: e.target.value }))}
              className="input"
              placeholder="Subtask title"
              required
              autoFocus
            />
            
            <textarea
              value={formData.description}
              onChange={(e) => setFormData(prev => ({ ...prev, description: e.target.value }))}
              className="input min-h-[60px] resize-none"
              placeholder="Subtask description"
            />
            
            <div className="flex gap-2">
              <button
                type="submit"
                disabled={isCreating || !formData.title.trim()}
                className="btn-primary text-sm disabled:opacity-50"
              >
                {isCreating ? 'Creating...' : 'Add Subtask'}
              </button>
              <button
                type="button"
                onClick={() => setShowCreateForm(false)}
                className="btn-secondary text-sm"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      ) : (
        <button
          onClick={() => setShowCreateForm(true)}
          className="w-full p-3 text-left text-sm text-gray-500 hover:text-gray-700 hover:bg-gray-50 rounded-lg border border-dashed border-gray-200 hover:border-gray-300 transition-colors"
        >
          + Add subtask
        </button>
      )}
    </div>
  );
}

interface SortableSubtaskItemProps {
  subtask: Subtask;
  editingId: number | null;
  editData: EditDataType;
  isUpdating: boolean;
  isDeleting: boolean;
  statusColors: Record<TodoStatus, string>;
  onEdit: (subtask: Subtask) => void;
  onUpdate: () => void;
  onDelete: (id: number) => void;
  onToggleComplete: (subtask: Subtask) => void;
  setEditingId: (id: number | null) => void;
  setEditData: (data: UpdateSubtaskRequest & { id: number }) => void;
  handleEditDataChange: (updater: (prev: EditDataType) => EditDataType) => void;
}

function SortableSubtaskItem({ 
  subtask, 
  editingId, 
  editData, 
  isUpdating, 
  isDeleting, 
  statusColors, 
  onEdit, 
  onUpdate, 
  onDelete, 
  onToggleComplete, 
  setEditingId, 
  setEditData,
  handleEditDataChange 
}: SortableSubtaskItemProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: subtask.id.toString() });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  };

  return (
    <div 
      ref={setNodeRef} 
      style={style}
      className={`bg-gray-50 rounded-lg p-4 ${isDragging ? 'opacity-50 shadow-xl scale-105' : ''}`}
    >
      {editingId === subtask.id ? (
        <div className="space-y-3">
          <input
            type="text"
            value={editData.title}
            onChange={(e) => handleEditDataChange(prev => ({ ...prev, title: e.target.value }))}
            className="input"
            placeholder="Subtask title"
          />
          
          <textarea
            value={editData.description}
            onChange={(e) => handleEditDataChange(prev => ({ ...prev, description: e.target.value }))}
            className="input min-h-[60px] resize-none"
            placeholder="Subtask description"
          />
          
          <select
            value={editData.status}
            onChange={(e) => handleEditDataChange(prev => ({ ...prev, status: parseInt(e.target.value) as TodoStatus }))}
            className="input"
          >
            <option value={TodoStatus.Pending}>Pending</option>
            <option value={TodoStatus.InProgress}>In Progress</option>
            <option value={TodoStatus.Completed}>Completed</option>
            <option value={TodoStatus.Cancelled}>Cancelled</option>
          </select>
          
          <div className="flex gap-2">
            <button
              onClick={onUpdate}
              disabled={isUpdating}
              className="btn-primary text-sm"
            >
              {isUpdating ? 'Saving...' : 'Save'}
            </button>
            <button
              onClick={() => setEditingId(null)}
              className="btn-secondary text-sm"
            >
              Cancel
            </button>
          </div>
        </div>
      ) : (
        <div className="flex items-start justify-between">
          <div className="flex items-start gap-3 flex-1">
            <button
              {...attributes}
              {...listeners}
              className="flex-shrink-0 p-1 hover:bg-gray-200 rounded cursor-grab active:cursor-grabbing transition-colors"
              aria-label="Drag to reorder"
            >
              <Bars3Icon className="w-4 h-4 text-gray-400" />
            </button>
            <button
              onClick={() => onToggleComplete(subtask)}
              className={`flex-shrink-0 w-5 h-5 rounded-full border-2 flex items-center justify-center transition-colors ${
                subtask.status === TodoStatus.Completed
                  ? 'bg-green-600 border-green-600 text-white'
                  : 'border-gray-300 hover:border-green-400'
              }`}
            >
              {subtask.status === TodoStatus.Completed && (
                <CheckIcon className="w-3 h-3" />
              )}
            </button>
            <div className="flex-1">
              <h4 className={`font-medium mb-1 ${
                subtask.status === TodoStatus.Completed
                  ? 'text-gray-500 line-through'
                  : 'text-gray-900'
              }`}>
                {subtask.title}
              </h4>
              <p className={`text-sm mb-2 ${
                subtask.status === TodoStatus.Completed
                  ? 'text-gray-400 line-through'
                  : 'text-gray-600'
              }`}>
                {subtask.description}
              </p>
              <div className="flex items-center gap-2">
                <span className={`px-2 py-1 rounded-full text-xs font-medium ${statusColors[subtask.status]}`}>
                  {TodoStatus[subtask.status]}
                </span>
              </div>
            </div>
          </div>
          
          <div className="flex gap-2 ml-4">
            <button
              onClick={() => onEdit(subtask)}
              className="text-blue-600 hover:text-blue-800 text-sm"
            >
              Edit
            </button>
            <button
              onClick={() => onDelete(subtask.id)}
              disabled={isDeleting}
              className="text-red-600 hover:text-red-800 text-sm disabled:opacity-50"
            >
              Delete
            </button>
          </div>
        </div>
      )}
    </div>
  );
}