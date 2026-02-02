import { ConfirmDialogConfig, ConfirmDialogResult, DialogComponent } from '@/shared';
import {
  ApplicationRef,
  ComponentRef,
  createComponent,
  EnvironmentInjector,
  Injectable,
} from '@angular/core';
import { filter, take } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DialogService {
  private activeDialogs: ComponentRef<DialogComponent>[] = [];
  private dialogQueue: Array<() => void> = [];
  private isProcessing = false;

  constructor(
    private appRef: ApplicationRef,
    private injector: EnvironmentInjector,
  ) {}

  confirm(
    message: string,
    config?: Partial<ConfirmDialogConfig>,
    variant?: 'info' | 'warning' | 'success' | 'error',
  ): Promise<ConfirmDialogResult> {
    return new Promise((resolve) => {
      const showDialog = () => {
        this.isProcessing = true;
        let resolved = false;

        const dialogConfig: ConfirmDialogConfig = {
          title: 'ยืนยันการดำเนินการ',
          message: message,
          confirmText: 'Yes',
          cancelText: 'No',
          showCancelButton: true,
          autoClose: false,
          duration: 10000,
          variant: variant || 'info',
          ...config,
        };

        const dialogRef = createComponent(DialogComponent, {
          environmentInjector: this.injector,
        });

        dialogRef.instance.title = dialogConfig.title || '';
        dialogRef.instance.message = dialogConfig.message;
        dialogRef.instance.confirmText = dialogConfig.confirmText || 'Yes';
        dialogRef.instance.cancelText = dialogConfig.cancelText || 'No';
        dialogRef.instance.showCancelButton = dialogConfig.showCancelButton !== false;
        dialogRef.instance.autoClose = dialogConfig.autoClose || false;
        dialogRef.instance.duration = dialogConfig.duration || 10000;
        dialogRef.instance.variant = dialogConfig.variant || 'info';

        const resultSubscription = dialogRef.instance.result.subscribe(
          (result: ConfirmDialogResult) => {
            if (!resolved) {
              resolved = true;
              resultSubscription.unsubscribe();
              resolve(result);
              this.closeDialog(dialogRef);
              this.processNextDialog();
            }
          },
        );

        this.attachDialogToDOM(dialogRef);
      };

      if (this.isProcessing) {
        this.dialogQueue.push(showDialog);
      } else {
        showDialog();
      }
    });
  }

  private processNextDialog(): void {
    this.isProcessing = false;

    if (this.dialogQueue.length > 0) {
      const nextDialog = this.dialogQueue.shift();
      if (nextDialog) {
        setTimeout(() => {
          nextDialog();
        }, 100);
      }
    }
  }

  private attachDialogToDOM(dialogRef: ComponentRef<DialogComponent>): void {
    this.appRef.attachView(dialogRef.hostView);
    const domElement = (dialogRef.hostView as any).rootNodes[0] as HTMLElement;

    let overlayContainer = document.querySelector(
      '.confirm-dialog-overlay-container',
    ) as HTMLElement;
    if (!overlayContainer) {
      overlayContainer = document.createElement('div');
      overlayContainer.className = 'confirm-dialog-overlay-container';
      overlayContainer.style.cssText = `
          position: fixed;
          top: 0;
          left: 0;
          width: 100%;
          height: 100%;
          pointer-events: none;
          z-index: 999999;
        `;
      document.body.appendChild(overlayContainer);
    }

    overlayContainer.appendChild(domElement);

    this.activeDialogs.push(dialogRef);

    this.appRef.isStable
      .pipe(
        filter((stable) => stable),
        take(1),
      )
      .subscribe(() => {
        dialogRef.instance.showDialog();
      });
  }

  confirmYesNo(
    message: string,
    title?: string,
    variant?: 'info' | 'warning' | 'success' | 'error',
  ): Promise<boolean> {
    return this.confirm(message, {
      title: title || 'ยืนยันการดำเนินการ',
      confirmText: 'Yes',
      cancelText: 'No',
      variant: variant || 'info',
    }).then((result) => result === 'yes');
  }

  confirmOkCancel(
    message: string,
    title?: string,
    variant?: 'info' | 'warning' | 'success' | 'error',
  ): Promise<boolean> {
    return this.confirm(message, {
      title: title || 'Confirmation',
      confirmText: 'OK',
      cancelText: 'Cancel',
      variant: variant || 'info',
    }).then((result) => result === 'yes');
  }

  confirmDelete(message: string = 'คุณต้องการลบรายการนี้หรือไม่?'): Promise<boolean> {
    return this.confirm(message, {
      title: 'ยืนยันการลบ',
      confirmText: 'ลบ',
      cancelText: 'ยกเลิก',
    }).then((result) => result === 'yes');
  }

  showInfo(message: string, title?: string): Promise<void> {
    return this.confirm(message, {
      title: title || 'Information Message',
      confirmText: 'OK',
      showCancelButton: false,
    }).then(() => {
      return;
    });
  }

  showError(message: string, title?: string): Promise<void> {
    return this.confirm(message, {
      title: title || 'Error Message',
      confirmText: 'OK',
      showCancelButton: false,
      variant: 'error',
    }).then(() => {
      return;
    });
  }

  showSuccess(message: string, title?: string): Promise<void> {
    return this.confirm(message, {
      title: title || 'Success Message',
      confirmText: 'OK',
      showCancelButton: false,
      variant: 'success',
    }).then(() => {
      return;
    });
  }

  showWarning(message: string, title?: string): Promise<void> {
    return this.confirm(message, {
      title: title || 'Warning Message',
      confirmText: 'OK',
      showCancelButton: false,
      variant: 'warning',
    }).then(() => {
      return;
    });
  }

  alert(message: string, title?: string): Promise<void> {
    return this.showInfo(message, title || 'แจ้งเตือน');
  }

  private closeDialog(dialogRef: ComponentRef<DialogComponent>): void {
    const index = this.activeDialogs.indexOf(dialogRef);
    if (index > -1) {
      this.activeDialogs.splice(index, 1);
    }

    this.appRef.detachView(dialogRef.hostView);
    dialogRef.destroy();

    if (this.activeDialogs.length === 0) {
      const overlayContainer = document.querySelector('.confirm-dialog-overlay-container');
      if (overlayContainer && overlayContainer.children.length === 0) {
        overlayContainer.remove();
      }
    }
  }

  closeAllDialogs(): void {
    this.activeDialogs.forEach((dialogRef) => {
      dialogRef.instance.hideDialog();
    });
  }
}
