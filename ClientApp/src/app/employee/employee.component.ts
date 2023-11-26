import { Component } from '@angular/core';
import {SupermarketService, SupermarketWithAddress} from "../service/store-service/store.service";
import {Router} from "@angular/router";
import {AuthenticationService} from "../service/auth-service/authentication.service";
import {Employee, EmployeeService} from "../service/employee-service/employee.service";

@Component({
  selector: 'app-employee',
  templateUrl: './employee.component.html',
  styleUrls: ['./employee.component.css']
})
export class EmployeeComponent {
  public employeeList: Employee[] = [];
  showSpinner = true;
  showButton = false;

  constructor(
    private router: Router,
    private employeeService: EmployeeService,
    private authService: AuthenticationService
  ) {
    this.getEmployees();
    setTimeout(() => {
      this.showSpinner = false;
      this.showButton = true;
    }, 1500);
  }

  getEmployees() {
    this.employeeService.getAllEmployees().subscribe(
      data => this.employeeList = data,
      error => console.error(error)
    );
  }

  deleteEmployee(id: number) {
    const ans = confirm("Do you want to delete the employee with ID: " + id);
    if (ans) {
      this.employeeService.deleteEmployee(id).subscribe(
        () => this.getEmployees(),
        error => console.error(error)
      );
    }
  }

  public isUserAuthenticated(): boolean {
    return this.authService.isUserSignedIn();
  }
}
