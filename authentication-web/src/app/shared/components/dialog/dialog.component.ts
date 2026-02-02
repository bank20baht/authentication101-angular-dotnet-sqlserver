import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

export type ConfirmDialogResult = 'yes' | 'no' | 'cancel';

export interface ConfirmDialogConfig {
  title?: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  showCancelButton?: boolean;
  autoClose?: boolean;
  duration?: number;
  variant?: 'info' | 'warning' | 'success' | 'error';
}

@Component({
  selector: 'authen-dialog',
  imports: [CommonModule],
  templateUrl: './dialog.component.html',
  styleUrl: './dialog.component.css',
})
export class DialogComponent {
  @Input() title: string = 'ยืนยันการดำเนินการ';
  @Input() message: string = '';
  @Input() confirmText: string = 'Yes';
  @Input() cancelText: string = 'No';
  @Input() showCancelButton: boolean = true;
  @Input() autoClose: boolean = false;
  @Input() duration: number = 10000;

  @Input() variant: 'info' | 'warning' | 'success' | 'error' = 'info';

  variantClassMap: Record<'info' | 'warning' | 'success' | 'error', string> = {
    info: 'bg-info',
    success: 'bg-success',
    warning: 'bg-warning',
    error: 'bg-error',
  };

  @Output() result = new EventEmitter<ConfirmDialogResult>();
  @Output() closed = new EventEmitter<void>();

  isVisible = false;
  private autoCloseTimer?: number;

  ngOnInit(): void {
    this.showDialog();
  }

  ngOnDestroy(): void {
    this.clearTimer();
  }

  showDialog(): void {
    this.isVisible = true;

    if (this.autoClose && this.duration > 0) {
      this.autoCloseTimer = window.setTimeout(() => {
        this.onCancel();
      }, this.duration);
    }
  }

  hideDialog(): void {
    this.isVisible = false;
    this.clearTimer();
    this.closed.emit();
  }

  onConfirm(): void {
    this.result.emit('yes');
    this.hideDialog();
  }

  onCancel(): void {
    this.result.emit('no');
    this.hideDialog();
  }

  getDialogClasses(): string {
    return `confirm-dialog ${this.isVisible ? 'dialog-visible' : 'dialog-hidden'}`;
  }

  private clearTimer(): void {
    if (this.autoCloseTimer) {
      clearTimeout(this.autoCloseTimer);
      this.autoCloseTimer = undefined;
    }
  }
}
