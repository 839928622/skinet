import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { IUserLogin } from 'src/app/shared/models/userLogin';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
 loginForm: FormGroup;
 user: IUserLogin;
  constructor(private accountService: AccountService,
              private router: Router) { }

  ngOnInit(): void {
    this.createLoginForm();
  }

  createLoginForm() {
    this.loginForm = new FormGroup({
      email: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required)
    });
  }

  onLoginFormSubmit() {
    this.user =   Object.assign({}, this.loginForm.value);
    this.accountService.login(this.user.email, this.user.password).subscribe( () => {
      this.router.navigateByUrl('/shop');
      console.log('login.components.ts', 'user logged in');
    }, error => {
      console.log(error);
    });
  }
}
