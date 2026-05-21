/** Represents the authenticated user as stored in localStorage. */
export interface User {
  userId: number;
  username: string;
  email: string;
  token: string;
}
