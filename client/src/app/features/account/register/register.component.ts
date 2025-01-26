import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { MatFormField, MatInput, MatLabel } from '@angular/material/input';
import { AccountService } from '../../../core/services/account.service';
import { Router } from '@angular/router';
import { SnakbarService } from '../../../core/services/snakbar.service';
import { TextInputComponent } from "../../../shared/components/text-input/text-input.component";

@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule,
    MatCard,
    MatButton,
    MatInput,
    MatLabel,
    MatFormField,
    TextInputComponent
],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private accountServices = inject(AccountService);
  private router = inject(Router);
  private snack = inject(SnakbarService);
  validationErrors? : string[];


  registerForm = this.fb.group({
    firstName : ['',Validators.required],
    LastName : ['',Validators.required],
    email : ['',[Validators.required,Validators.email]],
    password : ['',Validators.required],
  });

  onSubmit() {
    this.accountServices.register(this.registerForm.value).subscribe({
      next : ()=> {
        this.snack.success('Registeration Successful - you can now login');
        this.router.navigateByUrl('/account/login');
      },
      error : errors=> this.validationErrors = errors
    });
  }

}
