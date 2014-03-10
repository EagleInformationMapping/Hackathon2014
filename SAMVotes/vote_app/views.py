# Create your views here.

from django.views.generic import TemplateView


class MobileApp(TemplateView):
    template_name = 'mobile_app.html'
