import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'authen-button',
  imports: [CommonModule],
  templateUrl: './button.component.html',
  styleUrl: './button.component.css',
})
export class ButtonComponent {
  @Input() type: 'submit' | 'error' | 'primary' = 'submit';
  @Input() onDisabled: boolean = false;
  @Output() onClick = new EventEmitter<void>();

  get buttonClass() {
    return {
      'btn btn-submit': this.type === 'submit',
      'btn btn-error': this.type === 'error',
      'btn btn-primary': this.type === 'primary',
    };
  }
}
