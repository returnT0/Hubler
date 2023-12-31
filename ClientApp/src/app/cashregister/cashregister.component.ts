import { Component } from '@angular/core';
import { Router } from "@angular/router";
import { AuthenticationService } from "../service/auth-service/authentication.service";
import { CashRegisterData, CashRegisterService } from "../service/cashregister-service/cashregister.service";

@Component({
  selector: 'app-cashregister',
  templateUrl: './cashregister.component.html',
  styleUrls: ['./cashregister.component.css']
})
export class CashRegisterComponent {
  public cashRegisterList: CashRegisterData[] = [];
  showSpinner = true;
  showButton = false;

  constructor(
      private router: Router,
      private cashRegisterService: CashRegisterService,
      private authService: AuthenticationService
  ) {
    this.loadCashRegisters();
    setTimeout(() => {
      this.showSpinner = false;
      this.showButton = true;
    }, 1500);
  }

  loadCashRegisters() {
    this.cashRegisterService.getAll().subscribe(
        data => this.cashRegisterList = data,
        error => console.error(error)
    );
  }

  deleteCashRegister(cashRegister: CashRegisterData) {
    const confirmation = confirm(`Do you want to delete the cash register: ${cashRegister.registerNumber} at ${cashRegister.supermarketName}?`);
    if (confirmation) {
      this.cashRegisterService.delete(cashRegister.id).subscribe(
          () => this.loadCashRegisters(),
          error => console.error(error)
      );
    }
  }

  public isUserAuthenticated(): boolean {
    return this.authService.isUserSignedIn();
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  isCashier(): boolean {
    return this.authService.isCashier();
  }

  isManager(): boolean {
    return this.authService.isManager();
  }
}
