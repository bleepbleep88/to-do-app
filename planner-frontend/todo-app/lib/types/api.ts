// Based on the backend API DTOs
export interface User {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  isActive: boolean;
  createdAt: string;
  lastLoginAt: string;
}

export interface Subtask {
  id: number;
  title: string;
  description: string;
  status: TodoStatus;
  order: number;
  createdAt: string;
  updatedAt: string;
  todoId: number;
}

export interface Todo {
  id: number;
  title: string;
  description: string;
  status: TodoStatus;
  priority: Priority;
  dueDate: string | null;
  createdAt: string;
  updatedAt: string;
  userId: number;
  subtasks: Subtask[];
}

export enum TodoStatus {
  Pending = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3,
}

export enum Priority {
  Low = 0,
  Medium = 1,
  High = 2,
  Critical = 3,
}

// Request types
export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface CreateTodoRequest {
  title: string;
  description: string;
  priority: Priority;
  dueDate?: string | null;
}

export interface UpdateTodoRequest {
  title: string;
  description: string;
  status: TodoStatus;
  priority: Priority;
  dueDate?: string | null;
}

export interface CreateSubtaskRequest {
  title: string;
  description: string;
  order: number;
}

export interface UpdateSubtaskRequest {
  title: string;
  description: string;
  status: TodoStatus;
  order: number;
}

// Response types
export interface AuthResponse {
  token: string;
  user: User;
  expiresAt: string;
}

// API Error types
export interface ApiError {
  error: string;
  message?: string;
  statusCode?: number;
}

// Utility types
export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  limit: number;
  totalPages: number;
}

export interface ApiResponse<T = unknown> {
  data?: T;
  error?: ApiError;
  success: boolean;
}