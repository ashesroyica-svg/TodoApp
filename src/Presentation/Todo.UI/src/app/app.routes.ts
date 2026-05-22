import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { LoginComponent } from './views/auth/login/login.component';
import { RegisterComponent } from './views/auth/register/register.component';
import { TodoComponent } from './views/todo/todo.component';
import { DashboardComponent } from './views/dashboard/dashboard.component';
import { ProjectsComponent } from './views/projects/projects.component';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'auth/login', component: LoginComponent },
  { path: 'auth/register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'projects', component: ProjectsComponent, canActivate: [authGuard] },
  { path: 'todo', component: TodoComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: 'dashboard' }
];
