import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
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
 returnUrl: string;
  constructor(private accountService: AccountService,
              private router: Router,
              private activateRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.returnUrl = this.activateRoute.snapshot.queryParams.returnUrl || '/shop'; // if we dont have a returnUrl,then we redirect to shop
    this.createLoginForm();
  }

  createLoginForm() {
    this.loginForm = new FormGroup({
      email: new FormControl('', [Validators.required, Validators.pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$')]),
      password: new FormControl('', [Validators.required])
    });
  }

  onLoginFormSubmit() {
    if (this.loginForm.valid) {
      this.user =   Object.assign({}, this.loginForm.value);
      this.accountService.login(this.user.email, this.user.password).subscribe( () => {
        this.router.navigateByUrl(this.returnUrl);
        console.log('login.components.ts', 'user logged in');
      }, error => {
        console.log(error);
      });
  }
  }
}
