# Create your views here.
from twilio.twiml import Response
from django_twilio.decorators import twilio_view
from django.views.generic import TemplateView


class MobileApp(TemplateView):
    template_name = 'mobile_app.html'

# @twilio_view
# def reply_to_sms_messages(request):
#     r = Response()
#     r.sms('Thanks for the SMS message!')
#     return r