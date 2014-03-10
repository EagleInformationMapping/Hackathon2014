from django.db import models

class User(models.Model):

    time = models.DateTimeField()
    number = models.IntegerField()
    body = models.CharField(max_length=50)


    def __unicode__(self):
        return self.body

    # class Meta:
    #     db_table = 'pipeline'
    #     managed = False
