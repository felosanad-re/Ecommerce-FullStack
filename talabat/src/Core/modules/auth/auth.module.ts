import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputGroupAddonModule } from 'primeng/inputgroupaddon';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MessagesModule } from 'primeng/messages';
import { ToastModule } from 'primeng/toast';
import { NgxSpinnerModule } from 'ngx-spinner';
import { AutoFocusModule } from 'primeng/autofocus';
import { RouterLink } from '@angular/router';

@NgModule({
  declarations: [],
  imports: [
    FormsModule,
    InputGroupModule,
    InputGroupAddonModule,
    InputTextModule,
    ReactiveFormsModule,
    ButtonModule,
    MessagesModule,
    NgxSpinnerModule,
    AutoFocusModule,
    RouterLink,
  ],
  exports: [
    FormsModule,
    InputGroupModule,
    InputGroupAddonModule,
    InputTextModule,
    ReactiveFormsModule,
    ButtonModule,
    MessagesModule,
    NgxSpinnerModule,
    AutoFocusModule,
    RouterLink,
  ],
})
export class AuthModule {}
