# Create your views here.
from django.http import HttpResponse
from django.conf import settings
from django.views.generic import TemplateView
from twilio.rest import TwilioRestClient
from models import User
import json
import datetime
from django.views.generic import TemplateView
from httplib2 import Http
import proxy.views
from django.views.decorators.http import require_http_methods
from django.views.decorators.csrf import csrf_exempt

def load_test(request):
    return HttpResponse("loaderio-5aae65f5b1da419918b14ce0624c494d")#, 'text/plain')

def _get_token():
    return 'ghCA6D98-0GMrN0XgButJzZVek1jC-WOZ0Cw5aiaurvpuHPqSgeX54xK88Ds9bNoQWAlGRSbIOUe-RSIleKr88G_FsNDXJV1ysHTjc3aMZoWetQRCtOtUvslpaRtGJyF6KgKFH53o3tMZfja1ARvmg..'

@csrf_exempt
@require_http_methods(["GET", "POST"])
def proxy_view(request, path):
    #extra_requests_args = {'params': request.GET.copy()}
    #extra_requests_args = {'data': request.POST.copy()}
    extra_requests_args = {'params': request.GET.copy(), 'data': request.POST.copy()}
    #request.method = 'POST'
    #extra_requests_args['params']['token'] = _get_token()
    print(extra_requests_args)
    print(path)
    remoteurl = 'http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Network/USA/NAServer/Route/' + path
    #remoteurl = path
    # if remoteurl.find("?") > -1:
    #     remoteurl = remoteurl + "&token="+_get_token()
    # else:
    #     remoteurl = remoteurl + "?token="+_get_token()
    print(remoteurl)
    return proxy.views.proxy_view(request, remoteurl, extra_requests_args)

@csrf_exempt
@require_http_methods(["GET", "POST"])
def proxy_view_gp(request, path):
    extra_requests_args = {'params': request.GET.copy(), 'data': request.POST.copy()}
    #extra_requests_args = {'data': request.POST.copy()}
    #request.method = 'POST'
    #extra_requests_args['params']['token'] = _get_token()
    print(extra_requests_args)
    print(path)
    remoteurl = 'http://tasks.arcgisonline.com/ArcGIS/rest/services/Geometry/GeometryServer/' + path
    #remoteurl = path
    # if remoteurl.find("?") > -1:
    #     remoteurl = remoteurl + "&token="+_get_token()
    # else:
    #     remoteurl = remoteurl + "?token="+_get_token()
    print(remoteurl)
    return proxy.views.proxy_view(request, remoteurl, extra_requests_args)


class MobileApp(TemplateView):
    template_name = 'mobile_app.html'

    def get_context_data(self, **kwargs):
        context = super(MobileApp, self).get_context_data(**kwargs)
        return context

def reply_to_sms_messages(request):
    # r = Response()
    # r.sms('Thanks for the SMS message!')
    # t = TextPhone()
    # t.run('what up!!!;lkas;jfkadsjafjds;lka;kjag')
    # r = '<Response><Sms>Thanks for the SMS message!</Sms></Response>'
    # l = list()
    # for i in request.GET.keys():
    #     l.append(i)
    #     l.append(request.GET.get(i))

    u = User()


    # if len(User.objects.filter(number=request.GET.get('From'))) != 1:
    u.body = request.GET.get('Body')
    u.number = request.GET.get('From')
    u.time = datetime.datetime.now()
    u.save()
    l = request.GET.get('From')
    r = HttpResponse('<Response><Sms>Thanks! Check out your nearest polling location! {}</Sms></Response>'.format('http://hackathon2014.azurewebsites.net/?p={}'.format(l)), 'text/XML')
    # r = HttpResponse('<Response><Sms>Thank you for participating! '
    #                  'Stay tuned for your nearing polling '
    #                  'location!</Sms></Response>', 'text/XML')
    return r

def jamie_view(request):
    j_list = list()

    for i in User.objects.order_by('-time').all():
        temp = dict()
        temp['body'] = i.body
        temp['number'] = i.number
        j_list.append(temp)
    return HttpResponse(json.dumps(j_list), 'application/JSON')

def send_text(request):
    number = request.GET.get('n', None)
    body = request.GET.get('b', None)
    if number is None:
        return HttpResponse('')

    account_sid = settings.TWILIO_ACCOUNT_SID
    auth_token = settings.TWILIO_AUTH_TOKEN
    client = TwilioRestClient(account_sid, auth_token)

    message = client.messages.create(to=number, from_="51269",
                                     body=body)
    return HttpResponse('')