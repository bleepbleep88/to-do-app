'use client';

import { useState } from 'react';
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
} from '@dnd-kit/sortable';

import { useGetTodosQuery, useReorderTodosMutation } from '@/lib/api/todosApi';
import TodoItem from './TodoItem';
import CreateTodoForm from './CreateTodoForm';
import { useToast } from '@/components/ui/Toast';

export default function TodoList() {
  const { data: todos, isLoading, error } = useGetTodosQuery();
  const [reorderTodos] = useReorderTodosMutation();
  const { addToast } = useToast();
  
  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-8">
        <div className="text-gray-500">Loading todos...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-4 text-danger-600 bg-danger-50 border border-danger-200 rounded-lg">
        Failed to load todos. Please try again.
      </div>
    );
  }

  const handleDragEnd = async (event: DragEndEvent) => {
    const { active, over } = event;

    if (active.id !== over?.id && todos) {
      const oldIndex = todos.findIndex((todo) => todo.id.toString() === active.id);
      const newIndex = todos.findIndex((todo) => todo.id.toString() === over?.id);

      const reorderedTodos = arrayMove(todos, oldIndex, newIndex);
      
      // Send the new order to the backend
      try {
        await reorderTodos({
          todoIds: reorderedTodos.map(todo => todo.id)
        }).unwrap();
      } catch (error) {
        console.error('Failed to reorder todos:', error);
        addToast({
          type: 'error',
          title: 'Failed to reorder todos',
          message: 'Please try again',
        });
      }
    }
  };

  return (
    <div className="space-y-6">
      <CreateTodoForm />
      
      <div className="space-y-4">
        {todos && todos.length > 0 ? (
          <DndContext 
            sensors={sensors}
            collisionDetection={closestCenter}
            onDragEnd={handleDragEnd}
          >
            <SortableContext 
              items={todos.map(todo => todo.id.toString())}
              strategy={verticalListSortingStrategy}
            >
              {todos.map((todo) => (
                <TodoItem key={todo.id} todo={todo} />
              ))}
            </SortableContext>
          </DndContext>
        ) : (
          <div className="text-center py-8 text-gray-500">
            <p>No todos yet. Create your first todo above!</p>
          </div>
        )}
      </div>
    </div>
  );
}