# Create your views here.
# from twilio.twiml import Response
# from django_twilio.decorators import twilio_view
from django.http import HttpResponse
from django.views.generic import TemplateView
# from vote_app.test import TextPhone


class MobileApp(TemplateView):
    template_name = 'mobile_app.html'

# @twilio_view
def reply_to_sms_messages(request):
    # r = Response()
    # r.sms('Thanks for the SMS message!')
    # t = TextPhone()
    # t.run('what up!!!')
    # r = '<Response><Sms>Thanks for the SMS message!</Sms></Response>'
    r = HttpResponse('<Response><Sms>!working</Sms></Response>', 'text/XML')
    return r