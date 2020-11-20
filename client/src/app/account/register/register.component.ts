import { Component, OnInit } from '@angular/core';
import { AsyncValidatorFn, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { of, timer } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { IUserRegister } from 'src/app/shared/models/userRegister';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
registerForm: FormGroup;
userToLogin: IUserRegister;
errors: string[];
  constructor(private formBuider: FormBuilder,
              private accountService: AccountService,
              private router: Router, ) { }

  ngOnInit(): void {
    this.createRegisterForm();
  }

 createRegisterForm(): void {
   this.registerForm = this.formBuider.group({
     displayName: [null, [Validators.required]],
     email: [null, [Validators.required, Validators.pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$')]],
     password: [null, [Validators.required, Validators.pattern('^(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*)[0-9a-zA-Z]{6,}$')],
                      [this.validateEmailNotTaken()]] // async valitation will be called when sync validation are all passed above
   });
 }

 onRegisterFormSubmit(): void {
    if (this.registerForm.valid) {
      this.userToLogin =   Object.assign({}, this.registerForm.value);
      this.accountService.register(this.userToLogin.displayName, this.userToLogin.email, this.userToLogin.password).subscribe( () => {
        this.router.navigateByUrl('/shop');
        console.log('user registerd');
      }, error => {
        console.log(error);
        this.errors = error.errors;
      });
  }
  }

  validateEmailNotTaken(): AsyncValidatorFn{
    return control => {
      return timer(500).pipe(
        switchMap( () => {
          if (!control.value) {
            return of(null);
          }
          return this.accountService.checkEmailExists(control.value).pipe(
            map(res => {
              return res ? { emailExists: true } : null;
            })
          );
        })
      );
    };
  }
}


