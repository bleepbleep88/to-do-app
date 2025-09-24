import type { Metadata } from 'next';
import type { ReactNode } from 'react';
import { StoreProvider } from './StoreProvider';
import { ErrorBoundary } from '@/components/ui/ErrorBoundary';
import { ToastProvider } from '@/components/ui/Toast';

import './styles/globals.css';

export const metadata: Metadata = {
  title: 'Todo App',
  description: 'A modern todo application with subtasks',
};

interface Props {
  readonly children: ReactNode;
}

export default function RootLayout({ children }: Props) {
  return (
    <html lang="en">
      <body>
        <ErrorBoundary>
          <StoreProvider>
            <ToastProvider>
              {children}
            </ToastProvider>
          </StoreProvider>
        </ErrorBoundary>
      </body>
    </html>
  );
}
