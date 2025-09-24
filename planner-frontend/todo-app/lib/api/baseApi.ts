import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { config } from '@/lib/config';
import type { RootState } from '@/lib/store';

// Base API configuration
export const baseApi = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({
    baseUrl: config.api.baseUrl,
    prepareHeaders: (headers, { getState }) => {
      // Get token from Redux store
      const token = (getState() as RootState).auth.token;
      
      if (token) {
        headers.set('authorization', `Bearer ${token}`);
      }
      
      headers.set('content-type', 'application/json');
      
      return headers;
    },
  }),
  tagTypes: ['User', 'Todo', 'Subtask'],
  endpoints: () => ({}),
});

export const { 
  // Will be populated by individual API slices
} = baseApi;