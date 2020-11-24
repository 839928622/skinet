import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, of, ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IAddress } from '../shared/models/address';
import { IUser } from '../shared/models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
 baseUrl: string = environment.apiUrl;
 private currentUserSource = new ReplaySubject<IUser>(1); // the job of BehaviorSubject is to immediately emit its initial value
 currentUser$ = this.currentUserSource.asObservable();
  constructor(private httpClient: HttpClient,
              private router: Router) { }

  login(email: string, password: string): Observable<void> {
   return this.httpClient.post<IUser>(this.baseUrl + 'account/login', { Email: email, Password: password })
   .pipe(map((user: IUser) => {
     if (user) {
      localStorage.setItem('token', user.token);
      this.currentUserSource.next(user);
     }
   }));
  }

  register(displayName: string, email: string, password: string): Observable<void> {
    return this.httpClient.post(this.baseUrl + 'account/register', {DisplayName: displayName, Email: email, Password: password})
    .pipe(map((user: IUser) => {
      localStorage.setItem('token', user.token);
      this.currentUserSource.next(user);
    }));
  }

  logOut(): void {
    localStorage.removeItem('token');
    this.currentUserSource.next(null);
    this.router.navigate(['/']); // or this.router.navigateByUrl('/');
  }

  checkEmailExists(email: string): Observable<boolean> {
    return this.httpClient.get<boolean>(this.baseUrl + 'account/emailexists?email=' + email);
  }

  loadCurrentUser(token: string): Observable<void> {
   if (token === null){
     this.currentUserSource.next(null);
     return of(null);
   }

   let headers = new HttpHeaders();
   headers = headers.set('Authorization', `Bearer ${token}`);
   return this.httpClient.get<IUser>(this.baseUrl + 'account', {headers}).pipe(
      map((user: IUser) => {
          if (user) {
            localStorage.setItem('token', user.token);
            this.currentUserSource.next(user);
          }
      })
    );
  }

  getUserAddress(): Observable<IAddress>{
   return this.httpClient.get<IAddress>(this.baseUrl + 'account/address');
 }

 updateUserAddress(address: IAddress): Observable<IAddress> {
   return this.httpClient.post<IAddress>(this.baseUrl + 'account/address', address);
 }
}
