# Create your views here.
from django.http import HttpResponse
from django.views.generic import TemplateView
from models import User
import json
import datetime

class MobileApp(TemplateView):
    template_name = 'mobile_app.html'

def reply_to_sms_messages(request):
    # r = Response()
    # r.sms('Thanks for the SMS message!')
    # t = TextPhone()
    # t.run('what up!!!')
    # r = '<Response><Sms>Thanks for the SMS message!</Sms></Response>'
    # l = list()
    # for i in request.GET.keys():
    #     l.append(i)
    #     l.append(request.GET.get(i))

    u = User()
    u.body = request.GET.get('Body')
    u.number = request.GET.get('From')
    u.time = datetime.datetime.now()
    u.save()
    l = request.GET.get('From')
    r = HttpResponse('<Response><Sms>Your phone number is {}</Sms></Response>'.format(l), 'text/XML')
    return r

def jamie_view(request):
    j_list = list()

    for i in User.objects.order_by('time').all():
        temp = dict()
        temp['body'] = i.body
        temp['number'] = i.number
        j_list.append(temp)
    return HttpResponse(json.dumps(j_list), 'application/JSON')