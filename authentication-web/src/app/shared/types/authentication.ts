export type AuthorizationResponse = {
  username: string;
  application_function: string[];
};

export type AuthenticationRequestBody = {
  username: string;
  password: string;
};

export type RegisterRequestBody = {
  username: string;
  password: string;
};
export type LogoutResponseBodyDto = {
  message: string;
  timestamps: string;
};

export type userData = {
  username: string;
};

export type RefreshTokenRequestBody = {
  username: string;
};
