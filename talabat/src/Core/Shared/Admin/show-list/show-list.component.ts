import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { CommonModule } from '@angular/common';
import { TagModule } from 'primeng/tag';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { RippleModule } from 'primeng/ripple';

@Component({
  selector: 'app-show-list',
  standalone: true,
  imports: [
    TableModule,
    ToastModule,
    CommonModule,
    TagModule,
    DropdownModule,
    ButtonModule,
    InputTextModule,
    FormsModule,
    DialogModule,
    RippleModule,
  ],
  templateUrl: './show-list.component.html',
  styleUrl: './show-list.component.scss',
})
export class ShowListComponent<T extends { id: number | string }> {
  @Input({ required: true }) title!: string;
  @Input({ required: true }) data!: T[];
  @Output() edit = new EventEmitter<T>();
  @Output() delete = new EventEmitter<T>();
  @Output() add = new EventEmitter<T>();
  cloneRow: { [id: number | string]: T } = {}; // To Save last data before edit

  // For Dialog
  submitted: boolean = false;
  dataDialog: boolean = false;

  constructor() {}

  // open edit dialog
  onRowEditInit(item: T) {
    this.cloneRow[item.id] = { ...item };
  }

  // Save Edit
  onRowEditSave(item: T) {
    // هنا بنطلّع الصف المعدّل للـ parent عشان يتعامل مع الـ API
    this.edit.emit(item);

    // بعد الإرسال الناجح → بنمسح النسخة المؤقتة
    delete this.cloneRow[item.id];
  }

  deleteBrand(item: T) {
    this.delete.emit(item);
  }

  onRowEditCancel(item: T, rowIndex: number) {
    // نرجّع الصف للحالة القديمة
    this.data[rowIndex] = this.cloneRow[item.id];
    delete this.cloneRow[item.id];
  }

  // Add Product Dialog
  openNew() {
    this.submitted = false;
    this.dataDialog = true;
  }
  hideDialog() {
    this.dataDialog = false;
    this.submitted = false;
  }

  save(item: T) {
    this.submitted = true;
    this.add.emit(item);
    this.dataDialog = false;
  }
}
