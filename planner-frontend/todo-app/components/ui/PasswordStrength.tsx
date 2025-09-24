'use client';

import { CheckIcon, XMarkIcon } from '@heroicons/react/24/outline';
import { passwordRequirements } from '@/lib/validation';

interface PasswordStrengthProps {
  password: string;
  className?: string;
}

export default function PasswordStrength({ password, className = '' }: PasswordStrengthProps) {
  return (
    <div className={`space-y-2 ${className}`}>
      <p className="text-sm font-medium text-gray-700">Password requirements:</p>
      <div className="space-y-1">
        {passwordRequirements.map((requirement) => {
          const isValid = requirement.test(password);
          return (
            <div key={requirement.id} className="flex items-center gap-2">
              <div className={`flex-shrink-0 w-4 h-4 rounded-full flex items-center justify-center ${
                isValid 
                  ? 'bg-green-500 text-white' 
                  : 'bg-gray-200 text-gray-400'
              }`}>
                {isValid ? (
                  <CheckIcon className="w-3 h-3" />
                ) : (
                  <XMarkIcon className="w-3 h-3" />
                )}
              </div>
              <span className={`text-sm ${
                isValid ? 'text-green-600' : 'text-gray-500'
              }`}>
                {requirement.label}
              </span>
            </div>
          );
        })}
      </div>
    </div>
  );
}