# Create your views here.
import django_twilio
from django.views.generic import TemplateView


class MobileApp(TemplateView):
    template_name = 'mobile_app.html'


