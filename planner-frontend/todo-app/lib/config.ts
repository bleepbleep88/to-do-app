export const config = {
  api: {
    baseUrl: process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7024/api',
  },
  app: {
    name: process.env.NEXT_PUBLIC_APP_NAME || 'Todo App',
    version: process.env.NEXT_PUBLIC_APP_VERSION || '1.0.0',
  },
  features: {
    analytics: process.env.NEXT_PUBLIC_ENABLE_ANALYTICS === 'true',
    errorReporting: process.env.NEXT_PUBLIC_ENABLE_ERROR_REPORTING === 'true',
  },
  isDevelopment: process.env.NODE_ENV === 'development',
  isProduction: process.env.NODE_ENV === 'production',
} as const;

export type Config = typeof config;