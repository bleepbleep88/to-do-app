import { baseApi } from './baseApi';
import type {
  Todo,
  CreateTodoRequest,
  UpdateTodoRequest,
  Subtask,
  CreateSubtaskRequest,
  UpdateSubtaskRequest,
} from '@/lib/types';

export const todosApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    getTodos: builder.query<Todo[], void>({
      query: () => '/todos',
      providesTags: ['Todo'],
    }),
    
    getTodo: builder.query<Todo, number>({
      query: (id) => `/todos/${id}`,
      providesTags: (result, error, id) => [{ type: 'Todo', id }],
    }),
    
    createTodo: builder.mutation<Todo, CreateTodoRequest>({
      query: (todo) => ({
        url: '/todos',
        method: 'POST',
        body: todo,
      }),
      invalidatesTags: ['Todo'],
    }),
    
    updateTodo: builder.mutation<Todo, { id: number; todo: UpdateTodoRequest }>({
      query: ({ id, todo }) => ({
        url: `/todos/${id}`,
        method: 'PUT',
        body: todo,
      }),
      onQueryStarted: async ({ id, todo }, { dispatch, queryFulfilled }) => {
        // Optimistic update
        const patchResult = dispatch(
          todosApi.util.updateQueryData('getTodos', undefined, (draft) => {
            const existingTodo = draft.find((t) => t.id === id);
            if (existingTodo) {
              Object.assign(existingTodo, todo);
            }
          })
        );
        try {
          await queryFulfilled;
        } catch {
          patchResult.undo();
        }
      },
      invalidatesTags: (result, error, { id }) => [{ type: 'Todo', id }],
    }),
    
    deleteTodo: builder.mutation<void, number>({
      query: (id) => ({
        url: `/todos/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['Todo'],
    }),

    reorderTodos: builder.mutation<void, { todoIds: number[] }>({
      query: (data) => ({
        url: '/todos/reorder',
        method: 'POST',
        body: { todoIds: data.todoIds },
      }),
      onQueryStarted: async ({ todoIds }, { dispatch, queryFulfilled }) => {
        // Optimistic update
        const patchResult = dispatch(
          todosApi.util.updateQueryData('getTodos', undefined, (draft) => {
            // Reorder the todos based on the new order
            const reorderedTodos = todoIds.map(id => draft.find(t => t.id === id)).filter((todo): todo is NonNullable<typeof todo> => Boolean(todo));
            // Replace the draft with reordered todos, maintaining any todos not in the reorder list
            const otherTodos = draft.filter(t => !todoIds.includes(t.id));
            draft.length = 0;
            draft.push(...reorderedTodos, ...otherTodos);
          })
        );
        try {
          await queryFulfilled;
        } catch {
          patchResult.undo();
        }
      },
    }),
    
    // Subtask endpoints
    getSubtasks: builder.query<Subtask[], number>({
      query: (todoId) => `/todos/${todoId}/subtasks`,
      providesTags: (result, error, todoId) => [
        { type: 'Subtask', id: 'LIST' },
        { type: 'Todo', id: todoId },
      ],
    }),
    
    createSubtask: builder.mutation<Subtask, { todoId: number; subtask: CreateSubtaskRequest }>({
      query: ({ todoId, subtask }) => ({
        url: `/todos/${todoId}/subtasks`,
        method: 'POST',
        body: subtask,
      }),
      onQueryStarted: async ({ todoId, subtask }, { dispatch, queryFulfilled }) => {
        // Optimistic update
        const patchResult = dispatch(
          todosApi.util.updateQueryData('getTodos', undefined, (draft) => {
            const todo = draft.find((t) => t.id === todoId);
            if (todo) {
              if (!todo.subtasks) {
                todo.subtasks = [];
              }
              // Create a temporary subtask with a negative ID for optimistic update
              const tempSubtask = {
                id: Date.now() * -1, // Temporary negative ID
                ...subtask,
                todoId,
                status: 0, // TodoStatus.Pending
                createdAt: new Date().toISOString(),
                updatedAt: new Date().toISOString(),
              };
              todo.subtasks.push(tempSubtask);
            }
          })
        );
        try {
          const result = await queryFulfilled;
          // Replace the temporary subtask with the real one from the server
          dispatch(
            todosApi.util.updateQueryData('getTodos', undefined, (draft) => {
              const todo = draft.find((t) => t.id === todoId);
              if (todo?.subtasks) {
                const tempIndex = todo.subtasks.findIndex(s => s.id < 0);
                if (tempIndex !== -1) {
                  todo.subtasks[tempIndex] = result.data;
                }
              }
            })
          );
        } catch {
          patchResult.undo();
        }
      },
      invalidatesTags: (result, error, { todoId }) => [
        { type: 'Subtask', id: 'LIST' },
        { type: 'Todo', id: todoId },
      ],
    }),
    
    updateSubtask: builder.mutation<Subtask, { todoId: number; id: number; subtask: UpdateSubtaskRequest }>({
      query: ({ todoId, id, subtask }) => ({
        url: `/todos/${todoId}/subtasks/${id}`,
        method: 'PUT',
        body: subtask,
      }),
      onQueryStarted: async ({ todoId, id, subtask }, { dispatch, queryFulfilled }) => {
        // Optimistic update
        const patchResult = dispatch(
          todosApi.util.updateQueryData('getTodos', undefined, (draft) => {
            const todo = draft.find((t) => t.id === todoId);
            if (todo?.subtasks) {
              const existingSubtask = todo.subtasks.find((s) => s.id === id);
              if (existingSubtask) {
                Object.assign(existingSubtask, subtask);
              }
            }
          })
        );
        try {
          await queryFulfilled;
        } catch {
          patchResult.undo();
        }
      },
      invalidatesTags: (result, error, { todoId, id }) => [
        { type: 'Subtask', id },
        { type: 'Todo', id: todoId },
      ],
    }),
    
    deleteSubtask: builder.mutation<void, { todoId: number; id: number }>({
      query: ({ todoId, id }) => ({
        url: `/todos/${todoId}/subtasks/${id}`,
        method: 'DELETE',
      }),
      onQueryStarted: async ({ todoId, id }, { dispatch, queryFulfilled }) => {
        // Optimistic update
        const patchResult = dispatch(
          todosApi.util.updateQueryData('getTodos', undefined, (draft) => {
            const todo = draft.find((t) => t.id === todoId);
            if (todo?.subtasks) {
              todo.subtasks = todo.subtasks.filter(s => s.id !== id);
            }
          })
        );
        try {
          await queryFulfilled;
        } catch {
          patchResult.undo();
        }
      },
      invalidatesTags: (result, error, { todoId }) => [
        { type: 'Subtask', id: 'LIST' },
        { type: 'Todo', id: todoId },
      ],
    }),

    reorderSubtasks: builder.mutation<void, { todoId: number; subtaskIds: number[] }>({
      query: ({ todoId, subtaskIds }) => ({
        url: `/todos/${todoId}/subtasks/reorder`,
        method: 'POST',
        body: { subtaskIds },
      }),
      onQueryStarted: async ({ todoId, subtaskIds }, { dispatch, queryFulfilled }) => {
        // Optimistic update
        const patchResult = dispatch(
          todosApi.util.updateQueryData('getTodos', undefined, (draft) => {
            const todo = draft.find((t) => t.id === todoId);
            if (todo?.subtasks) {
              // Reorder the subtasks based on the new order
              const reorderedSubtasks = subtaskIds.map(id => 
                todo.subtasks!.find(s => s.id === id)
              ).filter(Boolean);
              todo.subtasks = reorderedSubtasks as any;
            }
          })
        );
        try {
          await queryFulfilled;
        } catch {
          patchResult.undo();
        }
      },
    }),
  }),
});

export const {
  useGetTodosQuery,
  useGetTodoQuery,
  useCreateTodoMutation,
  useUpdateTodoMutation,
  useDeleteTodoMutation,
  useReorderTodosMutation,
  useGetSubtasksQuery,
  useCreateSubtaskMutation,
  useUpdateSubtaskMutation,
  useDeleteSubtaskMutation,
  useReorderSubtasksMutation,
} = todosApi;