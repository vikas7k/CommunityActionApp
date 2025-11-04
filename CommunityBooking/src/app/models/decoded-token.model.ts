export interface DecodedToken {
  sub: string;     // email
  name: string;    // customer name
  role: string;    // "customer"
  jti?: string;    // token id (optional)
  exp?: number;    // expiry (epoch time)
  iat?: number;    // issued at (optional)
}
