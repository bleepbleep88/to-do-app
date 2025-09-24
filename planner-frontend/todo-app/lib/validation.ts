export interface ValidationError {
  field: string;
  message: string;
}

export function validateTodo(data: { title: string; description: string }): ValidationError[] {
  const errors: ValidationError[] = [];

  if (!data.title.trim()) {
    errors.push({ field: 'title', message: 'Title is required' });
  } else if (data.title.length > 200) {
    errors.push({ field: 'title', message: 'Title must be less than 200 characters' });
  }

  if (data.description && data.description.length > 1000) {
    errors.push({ field: 'description', message: 'Description must be less than 1000 characters' });
  }

  return errors;
}

export function validateSubtask(data: { title: string; description: string }): ValidationError[] {
  const errors: ValidationError[] = [];

  if (!data.title.trim()) {
    errors.push({ field: 'title', message: 'Title is required' });
  } else if (data.title.length > 200) {
    errors.push({ field: 'title', message: 'Title must be less than 200 characters' });
  }

  if (data.description && data.description.length > 500) {
    errors.push({ field: 'description', message: 'Description must be less than 500 characters' });
  }

  return errors;
}

export function validateEmail(email: string): boolean {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
}

export interface PasswordRequirement {
  id: string;
  label: string;
  test: (password: string) => boolean;
}

export const passwordRequirements: PasswordRequirement[] = [
  {
    id: 'length',
    label: 'At least 8 characters',
    test: (password) => password.length >= 8,
  },
  {
    id: 'uppercase',
    label: 'One uppercase letter',
    test: (password) => /[A-Z]/.test(password),
  },
  {
    id: 'number',
    label: 'One number',
    test: (password) => /\d/.test(password),
  },
];

export function validatePassword(password: string): ValidationError[] {
  const errors: ValidationError[] = [];

  if (!password) {
    errors.push({ field: 'password', message: 'Password is required' });
    return errors;
  }

  passwordRequirements.forEach((requirement) => {
    if (!requirement.test(password)) {
      errors.push({ field: 'password', message: `Password must have ${requirement.label.toLowerCase()}` });
    }
  });

  return errors;
}

export function isPasswordValid(password: string): boolean {
  return passwordRequirements.every((requirement) => requirement.test(password));
}