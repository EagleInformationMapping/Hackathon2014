from twilio.rest import TwilioRestClient

 
# Find these values at https://twilio.com/user/account
account_sid = 'AC74f157e03d4c6762554264b24d696d0f'
auth_token = 'c5573aee5281b22c682fccfe41a8baa8'
client = TwilioRestClient(account_sid, auth_token)
 
message = client.messages.create(to="+12816861001", from_="+18329248472",
                                     body="Hello there!")