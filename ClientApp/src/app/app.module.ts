import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';

import {AppComponent} from './app.component';
import {AppRoutingModule} from './app-routing.module';
import {SidebarComponent} from "./sidebar/sidebar.component";
import {HomeComponent} from "./home/home.component";
import {SignInComponent} from "./auth/sign-in/sign-in.component";
import {NavComponent} from "./nav/nav.component";
import {LandingComponent} from "./landing/landing.component";
import {SignUpComponent} from "./auth/sign-up/sign-up.component";
import {UserComponent} from "./user/user.component";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {ChartComponent} from "./chart/chart.component";
import {NgApexchartsModule} from "ng-apexcharts";
import {EmployeeComponent} from "./employee/employee.component";
import {StoreComponent} from "./store/store.component";
import {SpinnerComponent} from "./spinner/spinner.component";
import {AddStoreComponent} from "./store/add-store/add-store.component";

export function tokenGetter() {
    return localStorage.getItem("jwt");
}

@NgModule({
    imports: [
        BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
        HttpClientModule,
        FormsModule,
        ReactiveFormsModule,
        BrowserAnimationsModule,
        AppRoutingModule,
        NgApexchartsModule,
    ],
    declarations: [
        AppComponent,
        SidebarComponent,
        UserComponent,
        HomeComponent,
        SignInComponent,
        SignUpComponent,
        NavComponent,
        LandingComponent,
        ChartComponent,
        EmployeeComponent,
        StoreComponent,
        SpinnerComponent,
        AddStoreComponent
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
