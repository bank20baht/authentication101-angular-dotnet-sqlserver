import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'authen-forms-builder',
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './forms-builder.component.html',
  styleUrl: './forms-builder.component.css',
})
export class FormsBuilderComponent {
  @Input() fields: FormFieldConfig[] = [];

  @Output() formValueChange = new EventEmitter<any>();
  @Output() optionSelected = new EventEmitter<{ key: string; option: any }>();
  @Output() onFormValidStatusChange = new EventEmitter<boolean>();

  timestamp = Date.now();
  showPassword = false;

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  onFieldChange(key: string, value: any, type?: string): void {
    const field = this.fields.find((f) => f.key === key);
    if (!field) return;

    let nextValue = value;

    if (field.format) {
      nextValue = field.format(nextValue);
    }

    field.value = nextValue;

    this.validateField(field);

    this.formValueChange.emit(this.getFormData());
    this.isFormValid();
  }

  clearForm(): void {
    this.fields.forEach((field) => {
      field.value = null;
      field.error = null;
      field.touched = false;
    });

    this.formValueChange.emit(this.getFormData());
    this.onFormValidStatusChange.emit(false);
    this.timestamp = Date.now();
  }

  getFormData(): any {
    const formData: any = {};
    this.fields.forEach((field) => {
      formData[field.key] = field.value;
    });
    return formData;
  }

  onOptionSelected(key: string, option: any): void {
    this.optionSelected.emit({ key, option });
  }

  isFormValid(): boolean {
    const valid = this.fields.every((field) => this.validateField(field));
    this.onFormValidStatusChange.emit(valid);
    return valid;
  }

  validateField(field: FormFieldConfig): boolean {
    if (!field.validation) {
      field.error = null;
      return true;
    }

    const error = field.validation(field.value, this.fields);
    field.error = error;

    return !error;
  }

  isFieldValid(field: FormFieldConfig): boolean {
    return this.validateField(field);
  }

  updateField(key: string, updates: Partial<FormFieldConfig>): void {
    const field = this.fields.find((f) => f.key === key);
    if (field) {
      Object.assign(field, updates);
    }
  }

  markFieldAsTouched(field: FormFieldConfig): void {
    if (field) {
      field.touched = true;
    }
  }
}

export interface FormFieldConfig {
  key: string;
  label: string;
  type: 'text' | 'password';
  placeholder?: string;
  disabled?: boolean;
  errorDesc?: string;
  touched?: boolean;
  displayKey?: string;
  value?: any;
  error?: string | null;
  validation?: (value: any, fields: FormFieldConfig[]) => string | null;
  format?: (value: any) => any;
}
