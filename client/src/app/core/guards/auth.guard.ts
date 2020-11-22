import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from 'src/app/account/account.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private accounService: AccountService, private router: Router) {}
  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    return this.accounService.currentUser$.pipe( // currentUser$ is ReplaySubject, if nothing held by currentUser$,
      // code block inside of pipe won't execute unitil currentUser$ has a value
      // in Gurad,we dont need to subscribe it
      map(auth => {
        if (auth) {
          return true;
        }
        this.router.navigate(['account/login'], { queryParams: { returnUrl: state.url }});
      })
    );
  }

}
